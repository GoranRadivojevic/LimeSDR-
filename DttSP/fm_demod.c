/* fm_demod.c */

/* This file is part of a program that implements a Software-Defined Radio.

Copyright (C) 2004, 2005, 2006, 2007 by Frank Brickle, AB2KT and Bob McGwier, N4HY

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

#include <fm_demod.h>
#include <limits.h>
#include <cfloat>

const float WFMde_emphasisTimeEU = 1000*1e-6;  // 50 uS de-emphasis for FM wide broadcast for EU
const float WFMde_emphasisTimeUS = 150*1e-6;  // 75 uS de-emphasis for FM wide broadcast for US
const float NFMde_emphasisTime = 1500*1e-6;  // 520 uS de-emphasis for FM narrow ham radio


void FMDemod (FMD fm)		// yt7pwr
{
	int i;
	COMPLEX tmp,f;
	REAL mod;

	
	if(fm->average != fm->average)
		fm->average = 0.0;
	if(fm->deemphasisAvg != fm->deemphasisAvg)
		fm->deemphasisAvg = 0.0;
	if(fm->stereo_decoder->_deemphasisAvgL != fm->stereo_decoder->_deemphasisAvgL)
		fm->stereo_decoder->_deemphasisAvgL = 0.0;
	if(fm->stereo_decoder->_deemphasisAvgR != fm->stereo_decoder->_deemphasisAvgR)
		fm->stereo_decoder->_deemphasisAvgR = 0.0;

	for (i = 0; i < CXBsize (fm->ibuf); i++)
	{
		tmp = CXBdata(fm->ibuf, i);
		f = Cmul(CXBdata(fm->ibuf, i), Conjg(fm->previous));
        mod = Cmodulus(f);

		if (mod > 0.0f)
        {
			f = Cdiv(f, Cmplx(mod, 0));
        }

		CXBreal(fm->obuf, i) = Cargument(f);
		CXBimag(fm->obuf, i) = 0.0f;
        fm->previous = tmp;
    }

	// DC remove
    for (i = 0; i < CXBsize (fm->ibuf); i++)
	{
		fm->average += 0.01f * (CXBreal(fm->obuf, i) - fm->average);
		CXBreal(fm->obuf, i) -= fm->average;
		CXBimag(fm->obuf, i) = CXBreal(fm->obuf, i);
    }

	if(fm->wide_enabled)
	{
		ProcessStereo(fm->stereo_decoder, fm->obuf, fm->obuf, CXBsize (fm->obuf));
	}
	else
	{
		// Deemphasis
		for (i = 0; i < CXBsize (fm->obuf); i++)
		{
			fm->deemphasisAvg += fm->deemphasisAlpha * (CXBreal(fm->obuf, i) - fm->deemphasisAvg);
			CXBreal(fm->obuf, i) = CXBimag(fm->obuf, i) = fm->deemphasisAvg;
		}
	}
}

FMD newFMD (REAL samprate,
		REAL f_initial,
		REAL f_lobound,
		REAL f_hibound,
		REAL f_bandwid, int size, COMPLEX * ivec, COMPLEX * ovec, char *tag)
{
	FMD fm = (FMD) safealloc (1, sizeof (FMDDesc), tag);

	fm->SampleRate = samprate;
	fm->size = size;
	fm->ibuf = newCXB (size, ivec, tag);
	fm->obuf = newCXB (size, ovec, tag);
	fm->deemphasisAlpha = (REAL)(1.0 - exp(-1.0 / (fm->SampleRate * NFMde_emphasisTime)));
	fm->deemphasisAvg = 0.0;
	fm->average = 0.0;
	fm->stereo_decoder = newStereoDecoder(samprate, 1, size);

	return fm;
}

void FMDreload (FMD fm, REAL f_lobound, REAL f_hibound, REAL f_bandwid, int wide)		// yt7pwr
{
	fm->deemphasisAvg = 0.0;
	fm->average = 0.0;
	fm->stereo_decoder->_deemphasisAvgL = 0.0;
	fm->stereo_decoder->_deemphasisAvgR = 0.0;

	if(wide == 1)
		fm->deemphasisAlpha = (REAL)(1.0 - exp(-1.0 / (fm->SampleRate * WFMde_emphasisTimeEU)));	// EU
	else if(wide == 2)
		fm->deemphasisAlpha = (REAL)(1.0 - exp(-1.0 / (fm->SampleRate * WFMde_emphasisTimeUS)));	// US
	else
		fm->deemphasisAlpha = (REAL)(1.0 - exp(-1.0 / (fm->SampleRate * NFMde_emphasisTime)));		// ham radio NFM
}

void delFMD (FMD fm)
{
	if (fm)
	{
		delCXB (fm->ibuf);
		delCXB (fm->obuf);
		delStereDecoder(fm->stereo_decoder);
		safefree ((char *) fm);
	}
}