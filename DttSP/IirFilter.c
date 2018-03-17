/* IirFilter.c */

/*
Copyright (C) 2013 YT7PWR
*/

#include <fromsys.h>
#include <IirFilter.h>
#include <am_demod.h>

float ProcessIirFilter(IirFilter pilotFilter, float sample)
{
	float result;

	if(pilotFilter->_x1 != pilotFilter->_x1)
		pilotFilter->_x1 = 0.0;
	if(pilotFilter->_x2 != pilotFilter->_x2)
		pilotFilter->_x2 = 0.0;
	if(pilotFilter->_y1 != pilotFilter->_y1)
		pilotFilter->_y1 = 0.0;
	if(pilotFilter->_y2 != pilotFilter->_y2)
		pilotFilter->_y2 = 0.0;
	
	result = pilotFilter->_b0 * sample + 
		pilotFilter->_b1 * pilotFilter->_x1 +
		pilotFilter->_b2 * pilotFilter->_x2	-
		pilotFilter->_a1 * pilotFilter->_y1 - 
		pilotFilter->_a2 * pilotFilter->_y2;
    pilotFilter->_x2 = pilotFilter->_x1;
    pilotFilter->_x1 = sample;
    pilotFilter->_y2 = pilotFilter->_y1;
    pilotFilter->_y1 = result;

    return result;
}

IirFilter IirFilterInit(IirFilterType filterType, double frequency, double sampleRate, int qualityFactor)
        {
			float w0;
			float alpha;

			//AMD am = (AMD)safealloc(1, sizeof(AMDDesc), "AMD proba");
			IirFilter filter = (IirFilter) safealloc(1, sizeof(IirFilterDesc), "Pilot IirFilter");

            w0 = 2.0 * M_PI * frequency / sampleRate;
            alpha = sin(w0) / (2.0f * qualityFactor);
            
            switch (filterType)
            {
                case LowPass:
                    filter->_b0 = (float) ((1.0 - cos(w0)) / 2.0);
                    filter->_b1 = (float) ( 1.0 - cos(w0));
                    filter->_b2 = (float) ((1.0 - cos(w0)) / 2.0);
                    filter->_a0 = (float) ( 1.0 + alpha);
                    filter->_a1 = (float) (-2.0 * cos(w0));
                    filter->_a2 = (float) ( 1.0 - alpha);
                    break;

                case HighPass:
                    filter->_b0 = (float) ( (1.0 + cos(w0)) / 2.0);
                    filter->_b1 = (float) (-(1.0 + cos(w0)));
                    filter->_b2 = (float) ( (1.0 + cos(w0)) / 2.0);
                    filter->_a0 = (float) (  1.0 + alpha);
                    filter->_a1 = (float) ( -2.0 * cos(w0));
                    filter->_a2 = (float) (  1.0 - alpha);
                    break;

                //case BandPass:
                default:
                    filter->_b0 = (float) (alpha);
                    filter->_b1 = 0.0f;
                    filter->_b2 = (float) (-alpha);
                    filter->_a0 = (float) ( 1.0 + alpha);
                    filter->_a1 = (float) (-2.0 * cos(w0));
                    filter->_a2 = (float) ( 1.0 - alpha);
                    break;

                case Notch:
                    filter->_b0 = 1.0f;
                    filter->_b1 = (float) (-2.0 * cos(w0));
                    filter->_b2 = 1.0f;
                    filter->_a0 = (float) ( 1.0 + alpha);
                    filter->_a1 = (float) (-2.0 * cos(w0));
                    filter->_a2 = (float) ( 1.0 - alpha);
                    break;
            }

            filter->_b0 /= filter->_a0;
            filter->_b1 /= filter->_a0;
            filter->_b2 /= filter->_a0;
            filter->_a1 /= filter->_a0;
            filter->_a2 /= filter->_a0;

            filter->_x1 = 0;
            filter->_x2 = 0;
            filter->_y1 = 0;
            filter->_y2 = 0;

			return filter;
        }