/* fm_stereo_decoder_h */

#ifndef _fm_stereo_decoder_h
#define _fm_stereo_decoder_h

#include <ovsv.h>
#include <filter.h>
#include <IirFilter.h>

typedef struct _StereoPll
{
    float _sampleRate;
    float _phase;
    float _frequencyRadian;
    float _minFrequencyRadian;
    float _maxFrequencyRadian;
    float _defaultFrequency;
    float _range;
    float _bandwidth;
    float _alpha;
    float _beta;
    float _zeta;
    float _phaseAdj;
    float _phaseAdjM;
    float _phaseAdjB;
    float _lockAlpha;
    float _lockOneMinusAlpha;
    float _lockTime;
    float _phaseErrorAvg;
    float _adjustedPhase;
    float _lockThreshold;
}StereoPllDesc, *StereoPll;

typedef struct _FMstereo_decoder
{
	StereoPll _pll;
    float* _pllBuffer;
    IirFilter _pilotFilter;
    float* _pilotFilterBuffer;
    CXB _channelABuffer_input;
    CXB _channelBBuffer_input;
	CXB _channelABuffer_output;
    CXB _channelBBuffer_output;
	FiltOvSv _channelAFilter;
    FiltOvSv _channelBFilter;
    double _sampleRate;
    int _audioDecimationFactor;
    float _deemphasisAlpha;
    float _deemphasisAvgL;
    float _deemphasisAvgR;
	BOOL StereoDetected;
	BOOL StereoEnabled; 
}StereoDecoderDesc, *StereoDecoder;

extern void ProcessStereo(StereoDecoder stereo_decoder, CXB baseband, CXB output, int length);
extern StereoDecoder newStereoDecoder(double sampleRate, int decimationStageCount, int size);
extern void ProcessPhaseError(StereoPll pll, float phaseError);
extern COMPLEX ProcessPll(StereoPll pll, float sample);
extern StereoPll newStereoPll(float sampRate);

#endif
