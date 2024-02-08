using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESG.ExpressionLib
{
    public class NamedPatterns
    {
        //	PLEASE READ:
        //	================================================================================
        //	Bits 9~12 of the 16-bit pattern enumeration identify the enumeration region. The
        //	lightbar region is 4096 bytes and all others are 512 bytes.
        //
        //	The enumeration region may be used by input devices to know what sequencers are
        //	online.
        //
        //	The first 25% of each region is reserved for nondescript patterns that are
        //	typically created by GUI.
        //
        //	Pattern enumeration zero (0) is reserved to stop a sequencer.
        //

        //	This region is for non-named indexed lightbar patterns.
        //	Index zero is always stops the token sequencer.
        //
        public const UInt16 Region_Base__Lightbar_Indexed_Patterns = 1;
        public const UInt16 Region_Size__Lightbar_Indexed_Patterns = 1023;

        //	This region is for named lightbar patterns.
        //
        public const UInt16 Region_Base__Lightbar_Named_Patterns = 1024;
        public const UInt16 Region_Size__Lightbar_Named_Patterns = 3072;


        //	This region is for non-named indexed Safety Director patterns.
        //	Index zero is always stops the token sequencer.
        //
        public const UInt16 Region_Base__SafetyDir_Indexed_Patterns = 4096;
        public const UInt16 Region_Size__SafetyDir_Indexed_Patterns = 128;

        //	This region is for named Safety Director patterns.
        //
        public const UInt16 Region_Base__SafetyDir_Named_Patterns = 4224;
        public const UInt16 Region_Size__SafetyDir_Named_Patterns = 384;


        //	This region is for non-named indexed Sound patterns.
        //	Index zero is always stops the token sequencer.
        //
        public const UInt16 Region_Base__Sound_Indexed_Patterns = 4608;
        public const UInt16 Region_Size__Sound_Indexed_Patterns = 128;

        //	This region is for named Sound patterns.
        //
        public const UInt16 Region_Base__Sound_Named_Patterns = 4736;
        public const UInt16 Region_Size__Sound_Named_Patterns = 384;


        //	This region is reserved.
        //
        public const UInt16 Region_Base__Reserved_Patterns = 5120;
        public const UInt16 Region_Size__Reserved_Patterns = 2560;


        //	This region is for non-named indexed misc. patterns.
        //	Index zero is always stops the token sequencer.
        //
        public const UInt16 Region_Base__Misc_Indexed_Patterns = 7680;
        public const UInt16 Region_Size__Misc_Indexed_Patterns = 128;

        //	This region is for named misc. patterns.
        //
        public const UInt16 Region_Base__Misc_Named_Patterns = 7808;
        public const UInt16 Region_Size__Misc_Named_Patterns = 384;



        //	PLEASE READ:
        //	================================================================================
        //	Bits 9~12 of the 16-bit pattern enumeration identify the enumeration region. The
        //	lightbar region is 4096 bytes and all others are 512 bytes.
        //
        //	The enumeration region may be used by input devices to know what sequencers are
        //	online.
        //
        //	The first 25% of each region is reserved for nondescript patterns that are
        //	typically created by GUI.
        //
        //	Pattern enumeration zero (0) is reserved to stop a sequencer.
        //
        public enum PatternCategory
        {
            PatternCategoryLightbar,
            PatternCategorySafetyDir,
            PatternCategorySound,
            PatternCategoryMisc,

        }

        /// <summary>
        /// ESG Pattern Enumerations.
        /// </summary>
        public enum Enum : UInt16
        {
            //	pattern enumeration zero is token sequencer stop 
            Pattern_Stop = 0,

            //	first indexed lightbar pattern
            Pattern_Lightbar_Indexed = Region_Base__Lightbar_Indexed_Patterns,

            //	named lightbar patterns
            Pattern_Lightbar_CycleAll = Region_Base__Lightbar_Named_Patterns,
            Pattern_Lightbar_Random,
            Pattern_Lightbar_Steady_PWM_50,
            Pattern_Lightbar_Double_75_FPM,
            Pattern_Lightbar_Title_13_Quad_65_FPM,
            Pattern_Lightbar_Title_13_Double_65_FPM,
            Pattern_Lightbar_Quint_Hold_75_FPM,
            Pattern_Lightbar_Pulse_8_Burst_75_FPM,
            Pattern_Lightbar_Reg_65_Single_120_FPM,
            Pattern_Lightbar_Reg_65_Double_120_FPM,
            Pattern_Lightbar_Reg_65_Triple_120_FPM,
            Pattern_Lightbar_Reg_65_Quad_120_FPM,
            Pattern_Lightbar_Reg_65_Burst_120_FPM,
            Pattern_Lightbar_Reg_65_Single_S_S_120_FPM,
            Pattern_Lightbar_Reg_65_Double_S_S_120_FPM,
            Pattern_Lightbar_Reg_65_Triple_S_S_120_FPM,
            Pattern_Lightbar_Reg_65_Quad_S_S_120_FPM,
            Pattern_Lightbar_Reg_65_Burst_S_S_120_FPM,
            Pattern_Lightbar_Quad_Alternate_S_S_150_FPM,
            Pattern_Lightbar_Quad_Cross_Alternate_150_FPM,
            Pattern_Lightbar_Double_Alternate_S_S_150_FPM,
            Pattern_Lightbar_Double_Cross_Alternate_150_FPM,
            Pattern_Lightbar_Quint_Hold_Alternate_S_S_150_FPM,
            Pattern_Lightbar_Quint_Hold_Cross_Alternate,
            Pattern_Lightbar_Quad_Alternate_S_S_150_FPM_Front,
            Pattern_Lightbar_Quad_Alternate_S_S_150_FPM_Rear,
            Pattern_Lightbar_Double_Alternate_S_S_150_FPM_Front,
            Pattern_Lightbar_Double_Alternate_S_S_150_FPM_Rear,
            Pattern_Lightbar_Quint_Hold_Alternate_Side_to_Side_Front,
            Pattern_Lightbar_Quint_Hold_Alternate_Side_to_Side_Rear,
            Pattern_Lightbar_Reg_65_Single_S_S_120_FPM_Center_Pulse,
            Pattern_Lightbar_Reg_65_Double_S_S_120_FPM_Center_Pulse,
            Pattern_Lightbar_Reg_65_Triple_S_S_120_FPM_Center_Pulse,
            Pattern_Lightbar_Reg_65_Quad_S_S_120_FPM_Center_Pulse,
            Pattern_Lightbar_Reg_65_Burst_S_S_120_FPM_Center_Pulse,
            Pattern_Lightbar_Quad_Alternate_S_S_150_FPM_Center_Pulse,
            Pattern_Lightbar_Double_Alternate_S_S_150_FPM_Center_Pulse,
            Pattern_Lightbar_Quint_Hold_Alternate_S_S_150_FPM_Center_Pulse,
            Pattern_Lightbar_Quad_Alternate_S_S_150_FPM_Front_Center_Pulse,
            Pattern_Lightbar_Quad_Alternate_S_S_150_FPM_Rear_Center_Pulse,
            Pattern_Lightbar_Double_Alternate_S_S_150_FPM_Front_Center_Pulse,
            Pattern_Lightbar_Double_Alternate_S_S_150_FPM_Rear_Center_Pulse,
            Pattern_Lightbar_Quint_Hold_Alternate_Side_to_Side_Front_Center_Pulse,
            Pattern_Lightbar_Quint_Hold_Alternate_Side_to_Side_Rear_Center_Pulse,
            Pattern_Lightbar_Quad_Alternate_S_S_Middle_75_FPM,
            Pattern_Lightbar_Quad_Alternate_S_S_Middle_75_FPM_Center_Pulse,
            Pattern_Lightbar_Fast_Rotate,
            Pattern_Lightbar_Wave_Rotate,
            Pattern_Lightbar_Rotate_Quad,
            Pattern_Lightbar_Fast_Quad,


            //	first indexed Safety Director pattern
            Pattern_SafetyDir_Indexed = Region_Base__SafetyDir_Indexed_Patterns,

            //	named Safety Director patterns
            Pattern_SafetyDir_Left = Region_Base__SafetyDir_Named_Patterns,
            Pattern_SafetyDir_Left_Solid,
            Pattern_SafetyDir_Right,
            Pattern_SafetyDir_Right_Solid,
            Pattern_SafetyDir_Center_Out,
            Pattern_SafetyDir_Center_Out_Solid,
            Pattern_SafetyDir_Wig_Wag,
            Pattern_SafetyDir_Alternating,
            Pattern_SafetyDir_Quad_Flash,
            Pattern_SafetyDir_Alternating_Center_Pulse,
            Pattern_SafetyDir_Quad_Flash_Center_Pulse,
            Pattern_SafetyDir_LeftFillAndChase,
            Pattern_SafetyDir_RightFillAndChase,
            Pattern_SafetyDir_CenterFillAndChase,
            Pattern_SafetyDir_Indexed_Front_Pattern,
            Pattern_SafetyDir_Indexed_Rear_Pattern,
        }


        
        public enum PatternPrefixes : byte
        {
            PatternPrefix_PatternWithRepeats = 0xA0,
            PatternPrefix_PatternStepWithPeriod = 0xB0,
            PatternPrefix_PatternStepWithRepeatsOfNestedPattern = 0xC0,
            PatternPrefix_PatternStepWithAllOff = 0xD0,
            PatternPrefix_PatternSectionStartWithRepeats = 0xE0,
            PatternPrefix_PatternSectionEnd = 0xF0,
            PatternPrefix_Mask = 0xF0,

        }
        public const UInt16 PatternEnumPrefixMask = 0xE0;


    }
}
