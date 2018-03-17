/* fm_demod.h */

#ifndef _fm_demod_h
#define _fm_demod_h

#include <fromsys.h>
#include <banal.h>
#include <datatypes.h>
#include <bufvec.h>
#include <cxops.h>
#include <fm_stereo_decoder.h>

typedef struct _fm_demod
{
  int size;
  CXB ibuf, obuf;
  COMPLEX previous;
  float delta;
  float SampleRate;
  float deemphasisAlpha;
  float deemphasisAvg;
  float average;
  StereoDecoder stereo_decoder;
  int wide_enabled;
} FMDDesc, *FMD;

extern void FMDemod (FMD fm);
extern void FMDreload (FMD fm, float f_lobound, float f_hibound, float f_bandwid, int wide);		// yt7pwr
extern FMD newFMD (float samprate,
		   float f_initial,
		   float f_lobound,
		   float f_hibound,
		   float f_bandwid,
		   int size, COMPLEX * ivec, COMPLEX * ovec, char *tag);
extern void delFMD (FMD fm);

#ifndef TWOPI
#define TWOPI (2.0*M_PI)
#endif

#endif
