/////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Audiokinetic Wwise generated include file. Do not edit.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef __WWISE_IDS_H__
#define __WWISE_IDS_H__

#include <AK/SoundEngine/Common/AkTypes.h>

namespace AK
{
    namespace EVENTS
    {
        static const AkUniqueID GREETINGSFROMHELSINKI = 3698975080U;
        static const AkUniqueID PAUSEALL = 4091047182U;
        static const AkUniqueID PLAYBEEP = 3043762247U;
        static const AkUniqueID PLAYBREATHING = 3934384867U;
        static const AkUniqueID PLAYCHOIXMUSIC = 3899680803U;
        static const AkUniqueID PLAYCHOIXMUSICNOLOOP = 3397257820U;
        static const AkUniqueID PLAYENDMUSIC = 1149452795U;
        static const AkUniqueID PLAYEPILOGUE = 2664430727U;
        static const AkUniqueID PLAYFREUDAUDIO = 1437163987U;
        static const AkUniqueID PLAYFREUDAUDIOB = 2896494411U;
        static const AkUniqueID PLAYFREUDMUSIC = 2051861868U;
        static const AkUniqueID PLAYHEARTBEAT = 4284960165U;
        static const AkUniqueID PLAYINTRODUCTION1 = 3972833680U;
        static const AkUniqueID PLAYINTRODUCTION2 = 3972833683U;
        static const AkUniqueID PLAYKARLAUDIO = 3334910481U;
        static const AkUniqueID PLAYKARLAUDIOB = 4224340129U;
        static const AkUniqueID PLAYKARLMUSIC = 4236082638U;
        static const AkUniqueID PLAYOLDFREUD1 = 2922769081U;
        static const AkUniqueID PLAYOLDFREUD1_SINGLE = 3655101832U;
        static const AkUniqueID PLAYOLDFREUD2 = 2922769082U;
        static const AkUniqueID PLAYPROLOGUE = 1486414576U;
        static const AkUniqueID PLAYPROLOGUENOLOOP = 862099911U;
        static const AkUniqueID PLAYPROLOGUENOMUSIC = 3041392870U;
        static const AkUniqueID PLAYTITLEMUSIC = 718394488U;
        static const AkUniqueID RESUMEALL = 3240900869U;
        static const AkUniqueID STOPALL = 3086540886U;
        static const AkUniqueID STOPCHOIXMUSIC = 4071506729U;
        static const AkUniqueID STOPINTRODUCTION = 172570369U;
    } // namespace EVENTS

    namespace STATES
    {
        namespace AVERAGEEMOTION
        {
            static const AkUniqueID GROUP = 4214977261U;

            namespace STATE
            {
                static const AkUniqueID NEGATIVECALM = 2993330889U;
                static const AkUniqueID NEGATIVEEXCITED = 4220012722U;
                static const AkUniqueID POSITIVECALM = 1699528801U;
                static const AkUniqueID POSITIVEEXCITED = 3161219322U;
            } // namespace STATE
        } // namespace AVERAGEEMOTION

        namespace SEQUENCE
        {
            static const AkUniqueID GROUP = 311444866U;

            namespace STATE
            {
                static const AkUniqueID HYPNOSIS = 3204929648U;
                static const AkUniqueID SELECTION = 1973847113U;
            } // namespace STATE
        } // namespace SEQUENCE

    } // namespace STATES

    namespace SWITCHES
    {
        namespace AVERAGEVALENCE
        {
            static const AkUniqueID GROUP = 240302138U;

            namespace SWITCH
            {
                static const AkUniqueID NEGATIVE = 4219547688U;
                static const AkUniqueID NEUTRAL = 670611050U;
                static const AkUniqueID POSITIVE = 1192865152U;
            } // namespace SWITCH
        } // namespace AVERAGEVALENCE

        namespace INTENSITY
        {
            static const AkUniqueID GROUP = 2470328564U;

            namespace SWITCH
            {
                static const AkUniqueID FAST = 2965380179U;
                static const AkUniqueID MID = 1182670505U;
                static const AkUniqueID SLOW = 787604482U;
            } // namespace SWITCH
        } // namespace INTENSITY

    } // namespace SWITCHES

    namespace GAME_PARAMETERS
    {
        static const AkUniqueID AUDIOVOLUME = 1146284679U;
        static const AkUniqueID EXCITEMENTLEVEL = 1428440363U;
        static const AkUniqueID MUSICVOLUME = 2346531308U;
        static const AkUniqueID NEUROTHEREMINVOLUME = 1821539562U;
        static const AkUniqueID STARTDELAY = 703117816U;
        static const AkUniqueID VALENCELEVEL = 2275494527U;
    } // namespace GAME_PARAMETERS

    namespace BANKS
    {
        static const AkUniqueID INIT = 1355168291U;
        static const AkUniqueID CHOICE2SOUNDBANK = 854256009U;
        static const AkUniqueID EPIENDSOUNDBANK = 3192374405U;
        static const AkUniqueID FREUDSOUNDBANK = 1916941780U;
        static const AkUniqueID INTROSOUNDBANK = 886154420U;
        static const AkUniqueID KARLSOUNDBANK = 237236122U;
        static const AkUniqueID OPENPROSOUNDBANK = 1053698777U;
    } // namespace BANKS

    namespace BUSSES
    {
        static const AkUniqueID AMBISONIC_2_2_BUS = 3501690417U;
        static const AkUniqueID FREUD_HALLUCINATIONMUSIC_BUS = 602705761U;
        static const AkUniqueID HEADLOCKED_BUS = 1208828863U;
        static const AkUniqueID KARL_SPATIAL_HALLUCINATION_MUSIC_BUS = 512097367U;
        static const AkUniqueID MASTER_AUDIO_BUS = 3803692087U;
        static const AkUniqueID MASTER_SECONDARY_BUS = 805203703U;
        static const AkUniqueID NORMAL_MUSIC_BUS = 1753840839U;
        static const AkUniqueID SPATIAL_MUSIC_BUS = 1193277952U;
    } // namespace BUSSES

    namespace AUX_BUSSES
    {
        static const AkUniqueID HEADLOCKED_REVERB = 2327170656U;
        static const AkUniqueID KARLMUSIC_REVERB = 1359398863U;
        static const AkUniqueID MUSIC_REVERB = 2450185101U;
    } // namespace AUX_BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__
