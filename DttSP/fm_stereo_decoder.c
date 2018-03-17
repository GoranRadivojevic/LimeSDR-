/* fm_stereo_decoder.c */

/*
Copyright (C) 2013 YT7PWR
*/

#include <common.h>
#include <IirFilter.h>
#include <fm_stereo_decoder.h>
#include <limits.h>
#include <cfloat>

extern const float WFMde_emphasisTimeEU; // 50 uS de-emphasis for FM wide broadcast for EU
extern const float WFMde_emphasisTimeUS;  // 50 uS de-emphasis for FM wide broadcast for US
extern const float NFMde_emphasisTime;  // 50 uS de-emphasis for FM narrow ham radio

const int PilotFrequency = 19000;
const int PllRange = 20;
const int PllBandwith = 10;
const float PllThreshold = 1.0f;
const float PllLockTime = 0.5f; // sec
const float PllZeta = 0.707f;
const float _deemphasisTime = 50 * 1e-6f;
const float _pllPhaseAdjM = 0.0f;
const float _pllPhaseAdjB = -1.75f;

DttSP_EXP int FMStereoDetected (unsigned int thread, unsigned int subrx)
{
	int result = 0;
	sem_wait(&top[thread].sync.upd.sem);
	result = rx[thread]->fm.gen->stereo_decoder->StereoDetected;
	sem_post(&top[thread].sync.upd.sem);

	return result;
}

void ProcessStereo(StereoDecoder stereo_decoder, CXB baseBand, CXB interleavedStereo, int length)
{
	int i;
	float a,b;
	float pilot;

    // Filter L+R
	memcpy((COMPLEX*)CXBbase(stereo_decoder->_channelABuffer_input), (COMPLEX*)CXBbase(baseBand), 
		CXBsize(baseBand) * sizeof(COMPLEX));
	filter_OvSv(stereo_decoder->_channelAFilter);	// filter ChannelA

	if(!stereo_decoder->StereoEnabled)
	{
		// Process mono deemphasis
        for (i = 0; i < length; i++)
        {
			stereo_decoder->_deemphasisAvgL += stereo_decoder->_deemphasisAlpha * 
				(CXBreal(stereo_decoder->_channelABuffer_output, i) - stereo_decoder->_deemphasisAvgL);
			CXBreal(interleavedStereo, i) = CXBimag(interleavedStereo, i) = stereo_decoder->_deemphasisAvgL;
        }

        return;
	}
	else
	{
		// Demodulate L-R
		for (i = 0; i < length; i++)
		{
			pilot = ProcessIirFilter(stereo_decoder->_pilotFilter, CXBreal(baseBand, i));
			ProcessPll(stereo_decoder->_pll, pilot);
			CXBreal(stereo_decoder->_channelBBuffer_input, i) = CXBimag(stereo_decoder->_channelBBuffer_input, i) =
				CXBreal(baseBand, i) * sin((double)(stereo_decoder->_pll->_adjustedPhase * 2.0f));
		}
	}

	if (!(stereo_decoder->_pll->_phaseErrorAvg < stereo_decoder->_pll->_lockThreshold))
    {
		stereo_decoder->StereoDetected = FALSE;
		// Process mono deemphasis
        for (i = 0; i < length; i++)
        {
			stereo_decoder->_deemphasisAvgL += stereo_decoder->_deemphasisAlpha * 
				(CXBreal(stereo_decoder->_channelABuffer_output, i) - stereo_decoder->_deemphasisAvgL);
			CXBreal(interleavedStereo, i) = CXBimag(interleavedStereo, i) = stereo_decoder->_deemphasisAvgL;
        }

         return;
    }

	stereo_decoder->StereoDetected = TRUE;
    // Filter L-R
	filter_OvSv(stereo_decoder->_channelBFilter);	// filter ChannelB

    // Calculate Left and Right channels
    for (i = 0; i < length; i++)
    {
		a = CXBreal(stereo_decoder->_channelABuffer_output, i);
        b = 2.0f * CXBreal(stereo_decoder->_channelBBuffer_output, i);
		CXBreal(interleavedStereo, i) = (a + b);		// left channel
		CXBimag(interleavedStereo, i) = (a - b);		// right channel
    }

    // Process deemphasis
    for (i = 0; i < length; i++)
    {
		stereo_decoder->_deemphasisAvgL += stereo_decoder->_deemphasisAlpha * 
			(CXBreal(interleavedStereo, i) - stereo_decoder->_deemphasisAvgL);
		CXBreal(interleavedStereo, i) = stereo_decoder->_deemphasisAvgL;			// left after deemphasis

        stereo_decoder->_deemphasisAvgR += stereo_decoder->_deemphasisAlpha * 
			(CXBimag(interleavedStereo, i) - stereo_decoder->_deemphasisAvgR);
		CXBimag(interleavedStereo, i) = stereo_decoder->_deemphasisAvgR;			// right after deemphasis
    }
}

StereoDecoder newStereoDecoder(double sampleRate, int decimationStageCount, int size)
{
	double outputSampleRate;
	float* coefficients;
	ComplexFIR coefA;
	ComplexFIR coefB;
	float high_freq;
	float low_freq;

	StereoDecoder stereo_decoder = (StereoDecoder) safealloc(1, sizeof(StereoDecoderDesc), "StereoDecoder");
    stereo_decoder->_audioDecimationFactor = 1;
    stereo_decoder->_sampleRate = sampleRate;
	stereo_decoder->_pilotFilterBuffer = (float*)safealloc(1, sizeof(float)*size, "PilotFilter Buffer");
	stereo_decoder->_pilotFilter = IirFilterInit(BandPass, PilotFrequency, sampleRate, 500);
	stereo_decoder->_pll = newStereoPll(sampleRate);
	stereo_decoder->_pll->_sampleRate = (float)sampleRate;
	stereo_decoder->_pll->_defaultFrequency = PilotFrequency;
	stereo_decoder->_pll->_range = PllRange;
	stereo_decoder->_pll->_bandwidth = PllBandwith;
	stereo_decoder->_pll->_zeta = PllZeta;
	stereo_decoder->_pll->_phaseAdjM = _pllPhaseAdjM;
	stereo_decoder->_pll->_phaseAdjB = _pllPhaseAdjB;
	stereo_decoder->_pll->_lockTime = PllLockTime;
	stereo_decoder->_pll->_lockThreshold = PllThreshold;
	stereo_decoder->StereoEnabled = FALSE;
	stereo_decoder->StereoDetected = FALSE;
    outputSampleRate = sampleRate / stereo_decoder->_audioDecimationFactor;

	if(sampleRate < 48000)
		high_freq = 8000.0;
	else
		high_freq = 20000.0;

	low_freq = 20.0;
	coefA = newFIR_Bandpass_COMPLEX (low_freq, high_freq, sampleRate, uni[0].buflen + 1);
	coefB = newFIR_Bandpass_COMPLEX (low_freq, high_freq, sampleRate, uni[0].buflen + 1);
	stereo_decoder->_channelAFilter = newFiltOvSv (FIRcoef (coefA), FIRsize (coefA),
		uni[0].wisdom.bits);
	stereo_decoder->_channelBFilter = newFiltOvSv (FIRcoef (coefB), FIRsize (coefB),
		uni[0].wisdom.bits);
	delFIR_Bandpass_COMPLEX(coefA);
	delFIR_Bandpass_COMPLEX(coefB);

	stereo_decoder->_channelABuffer_input = newCXB (FiltOvSv_fetchsize (stereo_decoder->_channelAFilter),
		FiltOvSv_fetchpoint (stereo_decoder->_channelAFilter),
		"channelAFilter output");
	stereo_decoder->_channelABuffer_output = newCXB (FiltOvSv_storesize (stereo_decoder->_channelAFilter),
		FiltOvSv_storepoint (stereo_decoder->_channelAFilter),
			"channelAFilter input");
	stereo_decoder->_channelBBuffer_input = newCXB (FiltOvSv_fetchsize (stereo_decoder->_channelBFilter),
		FiltOvSv_fetchpoint (stereo_decoder->_channelBFilter),
		"channelBFilter input buffer");
	stereo_decoder->_channelBBuffer_output = newCXB (FiltOvSv_storesize (stereo_decoder->_channelBFilter),
		FiltOvSv_storepoint (stereo_decoder->_channelBFilter),
		"channelBFilter output");

    stereo_decoder->_deemphasisAlpha = (float) (1.0 - exp(-1.0 / (outputSampleRate * WFMde_emphasisTimeEU)));
    stereo_decoder->_deemphasisAvgL = 0.0;
    stereo_decoder->_deemphasisAvgR = 0.0;

	return stereo_decoder;
}

void delStereDecoder(StereoDecoder p)
{
	if(p)
	{
		delFiltOvSv(p->_channelAFilter);
		delFiltOvSv(p->_channelBFilter);
		safefree((char*)p->_channelABuffer_input);
		safefree((char*)p->_channelBBuffer_input);
		safefree((char*)p->_channelABuffer_output);
		safefree((char*)p->_channelBBuffer_output);
		safefree((char*)p->_pilotFilterBuffer);
		safefree((char*)p->_pllBuffer);
		safefree((char*)p);
	}
}

//////////////////////////// Pilot PLL //////////////////////////////

StereoPll newStereoPll(float sampRate)
{
	float norm;
	StereoPll pll = (StereoPll*)safealloc(1, sizeof(StereoPllDesc), "StereoDecoder PLL");
    pll->_sampleRate = sampRate;
    pll->_defaultFrequency = PilotFrequency;
    pll->_range = PllRange;
	pll->_bandwidth = PllBandwith;
    pll->_zeta = PllZeta;
    pll->_phaseAdjM = _pllPhaseAdjM;
    pll->_phaseAdjB = _pllPhaseAdjB;
    pll->_lockTime = PllLockTime;
    pll->_lockThreshold = PllThreshold;
	norm  = (float) (2.0 * M_PI / pll->_sampleRate);
    pll->_phase = 0.0f;
    pll->_frequencyRadian = pll->_defaultFrequency * norm;
    pll->_minFrequencyRadian = (pll->_defaultFrequency - pll->_range) * norm;
    pll->_maxFrequencyRadian = (pll->_defaultFrequency + pll->_range) * norm;
    pll->_alpha = 2.0f * pll->_zeta * pll->_bandwidth * norm;
    pll->_beta = (pll->_alpha * pll->_alpha) / (4.0f * pll->_zeta * pll->_zeta);
    pll->_phaseAdj = pll->_phaseAdjM * pll->_sampleRate + pll->_phaseAdjB;
    pll->_lockAlpha = (float) (1.0 - exp(-1.0 / (pll->_sampleRate * pll->_lockTime)));
    pll->_lockOneMinusAlpha = 1.0f - pll->_lockAlpha;
	return pll;
}

COMPLEX ProcessPll(StereoPll pll, float sample)
{
	COMPLEX osc;
	float phaseError;
	osc.re = cos(pll->_phase);
	osc.im = sin(pll->_phase);
    osc  = Cscl(osc, sample);
	phaseError = -atan2(osc.im, osc.re);

	if(phaseError == _FPCLASS_SNAN || phaseError == _FPCLASS_ND ||
		phaseError == _FPCLASS_NINF || phaseError == _FPCLASS_QNAN)
		phaseError = 0.0;

    ProcessPhaseError(pll, phaseError);

    return osc;
}

void ProcessPhaseError(StereoPll pll, float phaseError)
{
	pll->_frequencyRadian += pll->_beta * phaseError;

    if (pll->_frequencyRadian > pll->_maxFrequencyRadian)
    {
		pll->_frequencyRadian = pll->_maxFrequencyRadian;
    }
    else if (pll->_frequencyRadian < pll->_minFrequencyRadian)
    {
		pll->_frequencyRadian = pll->_minFrequencyRadian;
    }

    pll->_phaseErrorAvg = pll->_lockOneMinusAlpha * pll->_phaseErrorAvg + pll->_lockAlpha * phaseError * phaseError;
    pll->_phase += pll->_frequencyRadian + pll->_alpha * phaseError;
	pll->_phase =  fmodf(pll->_phase, (float)(2.0 * M_PI));
    pll->_adjustedPhase = pll->_phase + pll->_phaseAdj;
}