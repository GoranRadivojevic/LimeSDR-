/* correctIQ.c

This routine restores quadrature between arms of an analytic signal
possibly distorted by ADC hardware.

This file is part of a program that implements a Software-Defined Radio.

Copyright (C) 2004, 2005, 2006 by Frank Brickle, AB2KT and Bob McGwier, N4HY

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

The authors can be reached by email at

ab2kt@arrl.net
or
rwmcgwier@comcast.net

or by paper mail at

The DTTS Microwave Society
6 Kathleen Place
Bridgewater, NJ 08807
*/

#include <common.h>
#include <limits.h>


int IQdoit = 1;
int IQsuspended = 0;
int IQfixed = 0;
int IQBalanced = 0;

IQ
newCorrectIQ (REAL phase, REAL gain, REAL mu)
{
	IQ iq = (IQ) safealloc (1, sizeof (iqstate), "IQ state");
	iq->phase = phase;
	iq->RXphase = 0.0f;
	iq->gain = gain;
	iq->RXgain = 0.0f;
	iq->mu = mu;
	iq->leakage = 0.000000f;
	iq->MASK=15;
	iq->index=0;
	iq->w = (COMPLEX *)safealloc(16,sizeof(COMPLEX),"correctIQ w");
	iq->b = (COMPLEX *)safealloc(16,sizeof(COMPLEX),"correctIQ b");			// yt7pwr
	iq->y = (COMPLEX *)safealloc(16,sizeof(COMPLEX),"correctIQ y");
	iq->del = (COMPLEX *)safealloc(16,sizeof(COMPLEX),"correctIQ del");
	memset((void *)iq->w,0,16*sizeof(COMPLEX));
	memset((void *)iq->b,0,16*sizeof(COMPLEX));								// yt7pwr
	return iq;
}

void
delCorrectIQ (IQ iq)
{
	safefree((char *)iq->w);
	safefree((char *)iq->b);												// yt7pwr
	safefree((char *)iq->y);
	safefree((char *)iq->del);
	safefree ((char *) iq);
}

void
correctIQ (CXB sigbuf, IQ iq, BOOLEAN isTX, int subchan, SDRMODE mode)
{
	int i,j;
	REAL doit;
	COMPLEX tmp;
	if (IQdoit == 0) return;
	if (subchan == 0 || subchan == 1) doit = iq->mu;
	else doit = 0;
	j = 0;

	if(!isTX)
	{
		if(IQfixed)
			{
				if(iq->RXgain == 1.0 && iq->RXphase == 0.0)
					return;

				for (i = 0; i < CXBhave (sigbuf); i++)
				{
					CXBimag (sigbuf, i) += iq->RXphase * CXBreal (sigbuf, i);
					CXBreal (sigbuf, i) *= iq->RXgain;
				}
			}
		else
			{
				// if (subchan == 0) // removed so that sub rx's will get IQ correction
				for (i = 0; i < CXBhave (sigbuf); i++)
				{
					iq->del[iq->index] = Cclamp(CXBdata(sigbuf,i));
					iq->y[iq->index] = Cclamp(Cadd(iq->del[iq->index],Cmul(iq->w[iq->index],Conjg(iq->del[iq->index]))));
					iq->y[iq->index] = Cclamp(Cadd(iq->y[iq->index],Cmul(iq->w[iq->index],Conjg(iq->y[iq->index]))));
					tmp = Cclamp(Csub(iq->w[iq->index], Cscl(Cmul(iq->y[iq->index],iq->y[iq->index]), doit)));  // this is where the adaption happens

					if(tmp.re < 100000.0 || tmp.im < 100000.0 || !isNaN(tmp.re) || !isNaN(tmp.im) )
						{
							if(IQsuspended == 0)
								iq->w[iq->index] = tmp;
						}
					else
					{
						iq->w[iq->index].im = iq->b[0].im;		// reset
						iq->w[iq->index].re = iq->b[0].re;
					}

					CXBdata(sigbuf,i)=iq->y[iq->index];
					//iq->index = (iq->index+iq->MASK)&iq->MASK;

                  j++;

                  if (j == 128)
                  {
                      iq->index = (iq->index+iq->MASK)&iq->MASK;
                      j = 0;
                  }
				}
			}
		//fprintf(stderr, "w1 real: %g, w1 imag: %g\n", iq->w[1].re, iq->w[1].im); fflush(stderr); 
	}
	else
	{
		switch(mode)
		{
		case CWU:
		case CWL:
			for (i = 0; i < CXBhave (sigbuf); i++)
			{
				CXBimag (sigbuf, i) += iq->phase * CXBreal (sigbuf, i);
				CXBreal (sigbuf, i) *= 1 + ( 1 - iq->gain);		// inverted around zero?!
			}
			break;
			
		default:

			for (i = 0; i < CXBhave (sigbuf); i++)
			{
				CXBimag (sigbuf, i) += iq->phase * CXBreal (sigbuf, i);
				CXBreal (sigbuf, i) *= iq->gain;
			}
			break;
		}
	}
}

INLINE COMPLEX Cclamp(COMPLEX r)
	{
		COMPLEX z;
		
		z = r;

		if(z.re > 10)
			z.re = 10;
		else if(z.re < -10)
			z.re = -10;

		if(z.im > 10)
			z.im = 10;
		else if(z.im < -10)
			z.im = -10;

		return z;
	}
