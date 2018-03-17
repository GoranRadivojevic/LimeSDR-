/* fm_stereo_decoder_h */

#ifndef _iirfilter_h
#define _iirfilter_h

    typedef enum _IirFilterType
    {
        LowPass,
        HighPass,
        BandPass,
        Notch
    }IirFilterType;

    typedef struct _IirFilter
    {
        float _a0;
        float _a1;
        float _a2;
        float _b0;
        float _b1;
        float _b2;
        float _x1;
        float _x2;
        float _y1;
        float _y2;
	}IirFilterDesc, *IirFilter;

extern float ProcessIirFilter(IirFilter filter, float sample);
extern IirFilter IirFilterInit(IirFilterType filterType, double frequency, double sampleRate, int qualityFactor);

#endif