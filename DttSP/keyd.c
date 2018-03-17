/* keyd.c */
/*
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

//#include <linux/rtc.h>
#include <fromsys.h>
#include <banal.h>
#include <splitfields.h>
#include <datatypes.h>
#include <bufvec.h>
#include <cxops.h>
#include <ringb.h>
#include <oscillator.h>
#include <cwtones.h>
#include <pthread.h>
#include <semaphore.h>
#include <keyer.h>
#include <windows.h>
#include <MMsystem.h>
#include <spottone.h>
#include <sdrexport.h>
MMRESULT timerid = 0;

PRIVATE CRITICAL_SECTION CS_CW,CS_CWMON,UPDATE_OK,UPDATE_MON;
PRIVATE LPCRITICAL_SECTION cs_cw,cs_cwmon, update_ok,update_mon;

REAL SAMP_RATE = 48000.0;
// # times key is sampled per sec
// > 64 requires root on Linux
int key_poll_period = 1;
//#define RTC_RATE (64)

// # samples generated during 1 clock tick at RTC_RATE
//#define TONE_SIZE (SAMP_RATE / RTC_RATE)
unsigned int TONE_SIZE = 96;
unsigned int SIZEBUF = 2*768;
// ring buffer size; > 1 sec at this sr
//#define RING_SIZE (1<<020)
//#define RING_SIZE (1<<017)
#define RING_SIZE 2*1048576


KeyerState ks;
KeyerLogic kl;

static pthread_t play, key, timer;
sem_t clock_fired, keyer_started, poll_fired, monitor_fired;

ringb_float_t *lring, *rring;
ringb_float_t *mon_lring, *mon_rring;		// yt7pwr


CWToneGen gen;
CWToneGen mon;
static BOOLEAN playing = FALSE,monitor = FALSE, iambic = FALSE, bug = FALSE, 
	cw_ring_reset = FALSE,cw_mon_ring_reset = FALSE; // yt7pwr
static REAL wpm = 18.0, mon_freq = 600.0, freq = 600.0, rise = 10.0, fall = 10.0, gain = 0.0;


//------------------------------------------------------------

static int count = 0;

DttSP_EXP void
CWMonitorExchange (float *bufl, float *bufr, int nframes)
{
	size_t numsamps, bytesize = sizeof (float) * nframes;

	if (cw_mon_ring_reset)
	{
		size_t reset_size = (unsigned)nframes;
		cw_mon_ring_reset = FALSE;
		EnterCriticalSection (cs_cwmon);
		ringb_float_restart (mon_lring, reset_size);
		ringb_float_restart (mon_rring, reset_size);
		memset (bufl, 0, bytesize);
		memset (bufr, 0, bytesize);
		LeaveCriticalSection (cs_cwmon);
		return;
	}

/*	if ((numsamps = ringb_float_read_space (mon_lring)) < (size_t) nframes)
	{
		memset (bufl, 0, bytesize);
		memset (bufr, 0, bytesize);
		cw_mon_ring_reset = TRUE;
	}
	else*/
	{
		EnterCriticalSection (cs_cwmon);
		ringb_float_read (mon_lring, bufl, nframes);
		ringb_float_read (mon_rring, bufr, nframes);
		LeaveCriticalSection (cs_cwmon);
	}
}

DttSP_EXP void
CWtoneExchange (float *bufl, float *bufr, int nframes)
{
	size_t numsamps, bytesize = sizeof (float) * nframes;

	if (cw_ring_reset)
	{
		size_t reset_size = (unsigned)nframes;
		cw_ring_reset = FALSE;
		EnterCriticalSection (cs_cw);
		ringb_float_restart (lring, reset_size);
		ringb_float_restart (rring, reset_size);
		memset (bufl, 0, bytesize);
		memset (bufr, 0, bytesize);
		LeaveCriticalSection (cs_cw);
		return;
	}

/*	if ((numsamps = ringb_float_read_space (lring)) < (size_t) nframes)
	{
		memset (bufl, 0, bytesize);
		memset (bufr, 0, bytesize);
		cw_ring_reset = TRUE;
	}
	else*/
	{
		EnterCriticalSection (cs_cw);
		ringb_float_read (lring, bufl, nframes);
		ringb_float_read (rring, bufr, nframes);
		LeaveCriticalSection (cs_cw);
	}	
}

// generated tone -> output ringbuffer
void
send_tone (void)
{
/*	if (ringb_float_write_space (lring) < TONE_SIZE)
	{
		//fprintf(stdout, "send_tone: cw_ring_reset = TRUE\n"), fflush(stdout);
		cw_ring_reset = TRUE;
	}
	else*/
	{
		int i;
		EnterCriticalSection (cs_cw);
		correctIQ(gen->buf, tx[0].iqfix, TRUE, 0, tx[0].mode);				// yt7pwr fix!
		for (i = 0; i < gen->size; i++)
		{
			float l = (float) CXBreal (gen->buf, i),
			r = (float) CXBimag (gen->buf, i);
			ringb_float_write (lring, (float *) &l, 1);
			ringb_float_write (rring, (float *) &r, 1);
		}
		LeaveCriticalSection (cs_cw);
	}
}

// generated tone -> output ringbuffer
void
send_monitor_tone (void)
{
	size_t numsamps;

/*	if ((numsamps = ringb_float_write_space (mon_lring)) < (size_t) TONE_SIZE)
	{	
		cw_mon_ring_reset = TRUE;
	}
	else*/
	{
		int i;
		EnterCriticalSection (cs_cwmon);
		for (i = 0; i < mon->size; i++)
		{
			float r = (float) CXBreal (mon->buf, i),
				l = (float) CXBimag (mon->buf, i);
			ringb_float_write (mon_lring, (float *) &l, 1);
			ringb_float_write (mon_rring, (float *) &r, 1);
		}
		LeaveCriticalSection (cs_cwmon);
	}
}

// silence -> output ringbuffer
void
send_silence (void)
{
/*	if (ringb_float_write_space (lring) < TONE_SIZE)
	{
		//fprintf(stdout, "send_silence: cw_ring_reset = TRUE\n"), fflush(stdout);
		cw_ring_reset = TRUE;
	}
	else*/
	{
		int i;
		EnterCriticalSection (cs_cw);
		for (i = 0; i < gen->size; i++)
		{
			float zero = 0.0;
			ringb_float_write (lring, &zero, 1);
			ringb_float_write (rring, &zero, 1);
		}
		LeaveCriticalSection (cs_cw);
    }
}

// silence -> output ringbuffer
void
send_monitor_silence (void)
{
	size_t numsamps;

/*	if ((numsamps = ringb_float_write_space (mon_lring)) < (size_t) TONE_SIZE)
	{
		cw_mon_ring_reset = TRUE;
	}
	else*/
	{
		int i;
		EnterCriticalSection (cs_cwmon);
		for (i = 0; i < mon->size; i++)
		{
			float zero = 0.0;
			ringb_float_write (mon_lring, &zero, 1);
			ringb_float_write (mon_rring, &zero, 1);
		}
		LeaveCriticalSection (cs_cwmon);
    }
}

// sound/silence generation
// tone turned on/off asynchronously

CALLBACK
timer_callback (UINT uTimerID, UINT uMsg, DWORD_PTR dwUser, DWORD_PTR dw1,
		DWORD_PTR dw2)
{
	sem_post (&poll_fired);
}

DttSP_EXP void
sound_thread_keyd (void)
{
	for (;;)
	{
		sem_wait (&clock_fired);

		if (playing)
		{
            // CWTone keeps playing for awhile after it's turned off,
			// in order to allow for a decay envelope;
			// returns FALSE when it's actually done.
			playing = CWTone (gen);
			EnterCriticalSection(update_ok);
			send_tone ();
			LeaveCriticalSection(update_ok);
		}
		else
		{
			EnterCriticalSection(update_ok);
			send_silence ();
			// only let updates run when we've just generated silence
			LeaveCriticalSection(update_ok);
		}
	}

	pthread_exit (0);
}

DttSP_EXP void
monitor_thread_keyd (void)
{
	for (;;)
	{
		sem_wait (&monitor_fired);

		if (monitor)
		{
            // CWTone keeps playing for awhile after it's turned off,
			// in order to allow for a decay envelope;
			// returns FALSE when it's actually done.*/
			monitor = CWTone (mon);
			EnterCriticalSection(cs_cwmon);
			send_monitor_tone();
			LeaveCriticalSection(cs_cwmon);
		}
		else
		{
			EnterCriticalSection(cs_cwmon);
			send_monitor_silence();
			// only let updates run when we've just generated silence
			LeaveCriticalSection(cs_cwmon);
		}
	}

	pthread_exit (0);
}

BOOLEAN
read_key (REAL del, BOOLEAN dot, BOOLEAN dash)
{
	extern BOOLEAN read_straight_key (KeyerState ks, BOOLEAN keyed);
	extern BOOLEAN read_iambic_key (KeyerState ks, BOOLEAN dot,
				  BOOLEAN dash, KeyerLogic kl, REAL ticklen);

	if (bug)
	{
		if (dash)
			return read_straight_key (ks, dash);
		else
			return read_iambic_key (ks, dot, FALSE, kl, del);
	}
	if (iambic)
		return read_iambic_key (ks, dot, dash, kl, del);
	return read_straight_key (ks, dot | dash);
}

/// Main keyer function,  called by a thread in the C#
BOOLEAN dotkey = FALSE;
PRIVATE BOOLEAN INLINE
whichkey (BOOLEAN dot, BOOLEAN dash)
{
	if (dotkey)
		return dot;
	return dash;
}

DttSP_EXP void
SetWhichKey (BOOLEAN isdot)
{
	if (isdot)
		dotkey = TRUE;
	else
		dotkey = FALSE;
}

DttSP_EXP void
key_thread_process (REAL del, BOOLEAN dash, BOOLEAN dot, BOOLEAN keyprog)
{
	BOOLEAN keydown;
	extern BOOLEAN read_straight_key (KeyerState ks, BOOLEAN keyed);
	// read key; tell keyer elapsed time since last call
	if (!keyprog)
		keydown = read_key (del, dot, dash);
	else
		keydown = read_straight_key (ks, whichkey (dot, dash));

	if (!playing && keydown)
	{
		CWToneOn (gen);
		CWToneOn(mon);
		playing = TRUE;
		monitor = TRUE;
	}
	else if (playing && !keydown)
	{
		CWToneOff (gen);
		CWToneOff(mon);
	}

	sem_post (&clock_fired);
	sem_post (&monitor_fired);
}

DttSP_EXP BOOLEAN
KeyerPlaying ()
{
	return playing;
}

//------------------------------------------------------------------------


DttSP_EXP void
SetKeyerBug (BOOLEAN bg)
{
	EnterCriticalSection(update_ok);
	if (bg)
	{
		iambic = FALSE;
		ks->flag.mdlmdB = FALSE;
		ks->flag.memory.dah = FALSE;
		ks->flag.memory.dit = FALSE;
		bug = TRUE;
	}
	else
		bug = FALSE;
	LeaveCriticalSection(update_ok);
}

DttSP_EXP void
SetKeyerSpeed (REAL speed)
{
	EnterCriticalSection(update_ok);
	wpm = ks->wpm = speed;
	LeaveCriticalSection(update_ok);
}

DttSP_EXP void
SetKeyerWeight (int newweight)
{
	EnterCriticalSection(update_ok);
	ks->weight = newweight;
	LeaveCriticalSection(update_ok);
}

DttSP_EXP void
SetKeyerIambic (BOOLEAN setit)
{
	EnterCriticalSection(update_ok);
	if (setit)
	{
		iambic = TRUE;
		ks->flag.mdlmdB = TRUE;
		ks->flag.memory.dah = TRUE;
		ks->flag.memory.dit = TRUE;
	}
	else
	{
		iambic = FALSE;
		ks->flag.mdlmdB = FALSE;
		ks->flag.memory.dah = FALSE;
		ks->flag.memory.dit = FALSE;
	}
	LeaveCriticalSection(update_ok);
}

DttSP_EXP void
SetKeyerFreq (REAL newfreq)
{
	EnterCriticalSection(update_ok);
	freq = -newfreq;
	setCWToneGenVals (gen, gain, freq, rise, fall);
	LeaveCriticalSection(update_ok);
}

DttSP_EXP void
SetMonitorFreq (REAL newfreq)
{
	EnterCriticalSection(update_mon);
	mon_freq = -newfreq;
	setCWToneGenVals (mon, gain, mon_freq, rise, fall);
	LeaveCriticalSection(update_mon);
}

DttSP_EXP void
SetKeyerHarmonicRemove(double harmonic, double phase, double amplitude)
{
	EnterCriticalSection(update_ok);
	gen->amplitude =  amplitude;
	gen->harmonic = harmonic;
	gen->phase =  phase;
	LeaveCriticalSection(update_ok);
}

DttSP_EXP void
SetKeyerGain (REAL newgain)
{
	if ((newgain >= 0.0) && (newgain <= 1.0))
	{
		EnterCriticalSection(update_ok);
		gain = (REAL) (20.0 * log10 (newgain));
		setCWToneGenVals (gen, gain, freq, rise, fall);
		LeaveCriticalSection(update_ok);
	}
}

DttSP_EXP void
SetKeyerRise (REAL newrise)
{
	EnterCriticalSection(update_ok);
	rise = newrise;
	setCWToneGenVals (gen, gain, freq, rise, fall);
	setCWToneGenVals (mon, gain, mon_freq, rise, fall);
	LeaveCriticalSection(update_ok);
}

DttSP_EXP void
SetKeyerFall (REAL newfall)
{
	EnterCriticalSection(update_ok);
	fall = newfall;
	setCWToneGenVals (gen, gain, freq, rise, fall);
	setCWToneGenVals (mon, gain, mon_freq, rise, fall);
	LeaveCriticalSection(update_ok);
}

DttSP_EXP void SetKeyerMode (int newmode)
{
	EnterCriticalSection(update_ok);
	switch (newmode)
	{
		case 0:
			ks->mode = MODE_A;
			ks->flag.mdlmdB = FALSE;
			break;
		case 1:
			ks->mode = MODE_B;
			ks->flag.mdlmdB = TRUE;
			break;
		default:
			iambic = FALSE;
			break;
	}
	LeaveCriticalSection(update_ok);
}

DttSP_EXP void
SetKeyerDeBounce (int db)
{
	EnterCriticalSection(update_ok);
	ks->debounce = db;
	LeaveCriticalSection(update_ok);
}

DttSP_EXP void
SetKeyerRevPdl (BOOLEAN rvp)
{
	EnterCriticalSection(update_ok);
	ks->flag.revpdl = !rvp;
	LeaveCriticalSection(update_ok);
}

#if 0
DttSP_EXP void
SetKeyerPerf (BOOLEAN hiperf)
{
	MMRESULT tmp_timer;
	tmp_timer = timerid;
	if (timerid != 0)
	{
		EnterCriticalSection(update_ok);
		timeKillEvent ((UINT) timerid);
		timerid = 0;
		Sleep (11);
		LeaveCriticalSection(update_ok);
	}
	delCWToneGen (gen);

	key_poll_period = 1;
	TONE_SIZE = 48;

	gen = newCWToneGen (gain, freq, ramp, ramp, TONE_SIZE, SAMP_RATE);
	if (tmp_timer != 0)
	{
#ifndef INTERLEAVED
		EnterCriticalSection(cs_cw);
		ringb_float_restart (lring, SIZEBUF);
		ringb_float_restart (rring, SIZEBUF);
		LeaveCriticalSection(cs_cw);
#else
		ringb_float_restart (lring, SIZEBUF);
#endif
		if ((timerid =
			timeSetEvent (key_poll_period, 1,
			(LPTIMECALLBACK) timer_callback,
			(DWORD_PTR) NULL, TIME_PERIODIC)) == (MMRESULT) NULL)
		fprintf (stderr, "Timer failed\n"), fflush (stderr);      
    }
}
#endif // if 0

DttSP_EXP void
NewKeyer (REAL freq, BOOLEAN niambic, REAL gain, REAL newrise, REAL newfall, REAL wpm,
	  REAL SampleRate)
{
	BOOL out;
	kl = newKeyerLogic ();
	ks = newKeyerState ();
	ks->flag.iambic = niambic;
	ks->flag.revpdl = TRUE;	// depends on port wiring
	ks->flag.autospace.khar = ks->flag.autospace.word = FALSE;
	ks->flag.mdlmdB = TRUE;
	ks->flag.memory.dah = TRUE;
	ks->flag.memory.dit = TRUE;
	ks->debounce = 1;		// could be more if sampled faster
	ks->mode = MODE_B;
	ks->weight = 50;
	ks->wpm = wpm;
	iambic = niambic;
	cs_cw = &CS_CW;
	cs_cwmon = &CS_CWMON;
	out = InitializeCriticalSectionAndSpinCount (cs_cw, 0x00000080);
	out = InitializeCriticalSectionAndSpinCount (cs_cwmon, 0x00000080);
	update_ok = &UPDATE_OK;
	update_mon = &UPDATE_MON;
	out = InitializeCriticalSectionAndSpinCount (update_ok, 0x00000080);
	out = InitializeCriticalSectionAndSpinCount (update_mon, 0x00000080);
#ifndef INTERLEAVED
	lring = ringb_float_create (RING_SIZE);
	rring = ringb_float_create (RING_SIZE);
	mon_lring = ringb_float_create (RING_SIZE);
	mon_rring = ringb_float_create (RING_SIZE);
#else
	lring = ringb_float_create (2 * RING_SIZE);
	mon_lring = ringb_float_create (2 * RING_SIZE);
#endif
	sem_init (&clock_fired, 0, 0);
	sem_init (&monitor_fired, 0, 0);
	sem_init (&poll_fired, 0, 0);
	sem_init (&keyer_started, 0, 0);

	key_poll_period = 4;
	TONE_SIZE = 192 * (int) (uni[0].samplerate / 48000.0);
	SIZEBUF = 3072 * (int) (uni[0].samplerate / 48000.0);

	//------------------------------------------------------------
	rise = newrise;
	fall = newfall;
	SAMP_RATE = SampleRate;
	delCWToneGen(gen);
	gen = newCWToneGen (gain, freq, rise, fall, TONE_SIZE, SampleRate);
	gen->amplitude = 0;
	gen->phase = 0;
	gen->harmonic = 0;

	delCWToneGen(mon);
	mon = newCWToneGen (gain, mon_freq, rise, fall, TONE_SIZE, SampleRate);
	mon->amplitude = 0;
	mon->phase = 0;
	mon->harmonic = 0;

	//------------------------------------------------------------
	//  if (timeSetEvent(5,1,(LPTIMECALLBACK)timer_callback,(DWORD_PTR)NULL,TIME_PERIODIC) == (MMRESULT)NULL) {
	//        fprintf(stderr,"Timer failed\n"),fflush(stderr);
	//  }
}

DttSP_EXP void
*NewCriticalSection()
{
	LPCRITICAL_SECTION cs_ptr;
	cs_ptr = (LPCRITICAL_SECTION)safealloc(1,sizeof(CRITICAL_SECTION),"Critical Section");
	return (void *)cs_ptr;
}

DttSP_EXP void
DestroyCriticalSection(LPCRITICAL_SECTION cs_ptr)
{
	safefree((char *)cs_ptr);
}


DttSP_EXP void
CWRingRestart ()
{
	//fprintf(stdout, "CWRingRestart: cw_ring_reset = TRUE\n"), fflush(stdout);
	cw_ring_reset = TRUE;
	cw_mon_ring_reset = TRUE;
}

DttSP_EXP void
StartKeyer ()
{
	CWRingRestart();
	if ((timerid =
		timeSetEvent (key_poll_period, 1, (LPTIMECALLBACK) timer_callback,
		(DWORD_PTR) NULL, TIME_PERIODIC)) == (MMRESULT) NULL)
	{
		fprintf (stderr, "Timer failed\n"), fflush (stderr);
	} 
	else
		sem_post (&keyer_started);
}

DttSP_EXP void
StopKeyer ()
{
	EnterCriticalSection(update_ok);
	if (timerid)
		timeKillEvent ((UINT) timerid);
	LeaveCriticalSection(update_ok);
	timerid = 0;
}

DttSP_EXP BOOLEAN KeyerRunning ()
{
	return (timerid != 0);
}

DttSP_EXP void DeleteKeyer ()
{
	StopKeyer();
	if (clock_fired)
	{
		sem_destroy (&clock_fired);
		clock_fired = NULL;
	}
	if (monitor_fired)
	{
		sem_destroy (&monitor_fired);
		monitor_fired = NULL;
	}
	if (poll_fired)
	{
		sem_destroy (&poll_fired);
		poll_fired = NULL;
	}
	if (keyer_started)
	{
		sem_destroy (&keyer_started);
		keyer_started = NULL;
	}
	delCWToneGen (gen);
	delCWToneGen (mon);
	delKeyerState (ks);
	delKeyerLogic (kl);
#ifndef INTERLEAVED
	ringb_float_free (lring);
	ringb_float_free (rring);
	ringb_float_free (mon_lring);
	ringb_float_free (mon_rring);
#else
	ringb_float_free (lring);
	ringb_float_free (mon_lring);
#endif
	if (cs_cw)
	{
		DeleteCriticalSection (cs_cw);
		cs_cw = NULL;
	}
	if (cs_cwmon)
	{
		DeleteCriticalSection (cs_cwmon);
		cs_cwmon = NULL;
	}
}

DttSP_EXP void
KeyerClockFireWait ()
{
	sem_wait (&clock_fired);
}

DttSP_EXP void
KeyerClockFireRelease ()
{
	sem_post (&clock_fired);
}

DttSP_EXP void
KeyerStartedWait ()
{
	sem_wait (&keyer_started);
}

DttSP_EXP void
KeyerStartedRelease ()
{
	sem_post (&keyer_started);
}

DttSP_EXP void
PollTimerWait ()
{
	sem_wait (&poll_fired);
}

DttSP_EXP void
PollTimerRelease ()
{
	sem_post (&poll_fired);
}

DttSP_EXP void
SetKeyerResetSize (unsigned int sizer)
{
//	SIZEBUF = sizer;
	//fprintf(stdout, "SetKeyerResetSize: cw_ring_reset = TRUE\n"), fflush(stdout);
	cw_ring_reset = TRUE;
	cw_mon_ring_reset = TRUE;
}

DttSP_EXP void
SetKeyerSampleRate (REAL sr)
{
	int factor = (int) (sr / 48000.0f);

	key_poll_period = 4;
	TONE_SIZE = 192 * factor;
	SIZEBUF = 3072 * factor;

	delCWToneGen (gen);
	gen = newCWToneGen (gain, freq, rise, fall, TONE_SIZE, sr);
	delCWToneGen (mon);
	mon = newCWToneGen (gain, mon_freq, rise, fall, TONE_SIZE, sr);
}

//------------------------------------------------------------------------

LARGE_INTEGER t1;

DttSP_EXP double TimerRead()
{
    LARGE_INTEGER frequency;        // ticks per second
    LARGE_INTEGER t2;		        // ticks
    double elapsedTime;

    QueryPerformanceFrequency(&frequency);
    QueryPerformanceCounter(&t2);
    elapsedTime = (t2.QuadPart - t1.QuadPart) * 1000.0 / frequency.QuadPart;
	t1 = t2;

    return elapsedTime;
}