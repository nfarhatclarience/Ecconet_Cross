using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using System.Drawing;
using ESG.ExpressionLib;
using ESG.ExpressionLib.DataModels;


namespace ESG.ExpressionLib.Collections
{
    /// <summary>
    /// A flash pattern table preloaded with ten sample patterns.
    /// The quadrant lights used are indexed one-byte outputs 500-503.
    /// </summary>
    public static class NamedQuadFlashPatterns
    {
        //  expression area keys
        public const UInt16 KeyArea1 = 500;
        public const UInt16 KeyArea2 = 501;
        public const UInt16 KeyArea3 = 502;
        public const UInt16 KeyArea4 = 503;
        public const UInt16 KeyArea5 = 504;
        public const UInt16 KeyArea6 = 505;
        public const UInt16 KeyArea7 = 506;
        public const UInt16 KeyArea8 = 507;
        public const UInt16 KeyArea9 = 508;
        public const UInt16 KeyArea10 = 509;
        public const UInt16 KeyArea11 = 510;
        public const UInt16 KeyArea12 = 511;
        public const UInt16 KeyArea13 = 512;
        public const UInt16 KeyArea14 = 513;
        public const UInt16 KeyArea15 = 514;
        public const UInt16 KeyArea16 = 515;
        public const UInt16 KeyArea17 = 516;
        public const UInt16 KeyArea18 = 517;

        public const UInt16 ChaseSpeed = 200;
        public const UInt16 ArrowStickSpeed = 500;

        //	brightness abbreviations
        public const byte FPL_OFF = 0;
        public const byte FPL_MID = 50;
        public const byte FPL_ON = 100;

        #region Factories
        //  1-area dictionary
        static BindingList<Expression.Area> SingleArea() => new BindingList<Expression.Area>
            { new Expression.Area("Area",  1, KeyArea1, 0, null), };

        //  2-area dictionary
        static BindingList<Expression.Area> DoubleArea() => new BindingList<Expression.Area>
            { new Expression.Area("Area",  1, KeyArea1, 0, null), new Expression.Area("Area",  2, KeyArea2, 0, null), };

        //  4-area dictionary
        static BindingList<Expression.Area> FourAreas() => new BindingList<Expression.Area>
            {
                new Expression.Area("Front-Left Quadrant",  1, KeyArea1, 0, null),
                new Expression.Area("Front-Right Quadrant", 2, KeyArea2, 0, null),
                new Expression.Area("Rear-Right Quadrant",  3, KeyArea3, 0, null),
                new Expression.Area("Rear-Left Quadrant",   4, KeyArea4, 0, null),
            };

        //  6-area dictionary
        static BindingList<Expression.Area> SixAreas() => new BindingList<Expression.Area>
            {
                new Expression.Area("LED", 1, KeyArea1, 0, null),
                new Expression.Area("LED", 2, KeyArea2, 0, null),
                new Expression.Area("LED", 3, KeyArea3, 0, null),
                new Expression.Area("LED", 4, KeyArea4, 0, null),
                new Expression.Area("LED", 5, KeyArea5, 0, null),
                new Expression.Area("LED", 6, KeyArea6, 0, null),
            };

        //  18-area dictionary
        static BindingList<Expression.Area> EighteenAreas() => new BindingList<Expression.Area>
            {
                new Expression.Area("LED",  1,  KeyArea1,  0, null),
                new Expression.Area("LED",  2,  KeyArea2,  0, null),
                new Expression.Area("LED",  3,  KeyArea3,  0, null),
                new Expression.Area("LED",  4,  KeyArea4,  0, null),
                new Expression.Area("LED",  5,  KeyArea5,  0, null),
                new Expression.Area("LED",  6,  KeyArea6,  0, null),
                new Expression.Area("LED",  7,  KeyArea7,  0, null),
                new Expression.Area("LED",  8,  KeyArea8,  0, null),
                new Expression.Area("LED",  9,  KeyArea9,  0, null),
                new Expression.Area("LED",  10, KeyArea10, 0, null),
                new Expression.Area("LED",  11, KeyArea11, 0, null),
                new Expression.Area("LED",  12, KeyArea12, 0, null),
                new Expression.Area("LED",  13, KeyArea13, 0, null),
                new Expression.Area("LED",  14, KeyArea14, 0, null),
                new Expression.Area("LED",  15, KeyArea15, 0, null),
                new Expression.Area("LED",  16, KeyArea16, 0, null),
                new Expression.Area("LED",  17, KeyArea17, 0, null),
                new Expression.Area("LED",  18, KeyArea18, 0, null),
            };

        //  single area off
        static BindingList<Expression.Token> SingleAreaOff() => new BindingList<Expression.Token>
            { new Expression.Token(KeyArea1, 0), };

        //  single area mid
        static BindingList<Expression.Token> SingleAreaMid() => new BindingList<Expression.Token>
            { new Expression.Token(KeyArea1, 50), };

        //  single area on
        static BindingList<Expression.Token> SingleAreaOn() => new BindingList<Expression.Token>
            { new Expression.Token(KeyArea1, 100), };

        #endregion  //  factories


        /// <summary>
        /// Constructor.
        /// </summary>
        public static ExpressionCollection Build()
        {
            return new ExpressionCollection("Named Quad Flash Patterns",
                new BindingList<Expression>
                {
                    //  1  Steady at 40% level
                    new Expression("Steady 40%", (UInt16)NamedPatterns.Enum.Pattern_Lightbar_Steady_PWM_50,
                        0, 2, 0, 40,
                        SingleArea(),
                        new BindingList<Expression.Entry>
                        {
                            new Expression.Step(100, SingleAreaMid()),
                        }),

                    //  2  Reg 65 Double S-S 120 FPM
                    new Expression("Reg 65 Double SS 120 FPM", (UInt16)NamedPatterns.Enum.Pattern_Lightbar_Reg_65_Double_S_S_120_FPM,
                        0, 1, 0, 100,
                        DoubleArea(),
                        new BindingList<Expression.Entry>
                        {
                            new Expression.RepeatSectionStart(1),
                            new Expression.Step(70, new BindingList<Expression.Token>
                                { new Expression.Token(KeyArea1, 100) }),
                            new Expression.Step(38, new BindingList<Expression.Token>
                                { new Expression.Token(KeyArea1, 0) }),
                            new Expression.RepeatSectionEnd(),
                            new Expression.Step(70, new BindingList<Expression.Token>
                                { new Expression.Token(KeyArea1, 100) }),
                            new Expression.Step(300, new BindingList<Expression.Token>
                                { new Expression.Token(KeyArea1, 0) }),
                            new Expression.Step(70, new BindingList<Expression.Token>
                                { new Expression.Token(KeyArea2, 100) }),
                            new Expression.Step(38, new BindingList<Expression.Token>
                                { new Expression.Token(KeyArea2, 0) }),
                            new Expression.Step(70, new BindingList<Expression.Token>
                                { new Expression.Token(KeyArea2, 100) }),
                            new Expression.Step(300, new BindingList<Expression.Token>
                                { new Expression.Token(KeyArea2, 0) }),
                        }),

                    //  3  Title 13 Quad 65 FPM
                    new Expression("Title 13 Quad 65 FPM", (UInt16)NamedPatterns.Enum.Pattern_Lightbar_Title_13_Quad_65_FPM,
                        0, 1, 0, 100,
                        SingleArea(),
                        new BindingList<Expression.Entry>
                        {
                            new Expression.Step(115, SingleAreaOn()),
                            new Expression.Step(40,  SingleAreaOff()),
                            new Expression.Step(115, SingleAreaOn()),
                            new Expression.Step(40,  SingleAreaOff()),
                            new Expression.Step(115, SingleAreaOn()),
                            new Expression.Step(40,  SingleAreaOff()),
                            new Expression.Step(115, SingleAreaOn()),
                            new Expression.Step(340, SingleAreaOff()),
                        }),

                    //  4  Title 13 Double 65 FPM
                    new Expression("Title 13 Double 65 FPM", (UInt16)NamedPatterns.Enum.Pattern_Lightbar_Title_13_Double_65_FPM,
                        0, 1, 0, 100,
                        SingleArea(),
                        new BindingList<Expression.Entry>
                        {
                            new Expression.Step(230, SingleAreaOn()),
                            new Expression.Step(50,  SingleAreaOff()),
                            new Expression.Step(230, SingleAreaOn()),
                            new Expression.Step(410, SingleAreaOff()),
                        }),

                    //  5  Quint-hold 75 FPM
                    new Expression("Quint-hold 75 FPM", (UInt16)NamedPatterns.Enum.Pattern_Lightbar_Quint_Hold_75_FPM,
                        0, 1, 1, 100,
                        SingleArea(),
                        new BindingList<Expression.Entry>
                        {
                            new Expression.Step(30,  SingleAreaOn()),
                            new Expression.Step(20,  SingleAreaOff()),
                            new Expression.Step(30,  SingleAreaOn()),
                            new Expression.Step(20,  SingleAreaOff()),
                            new Expression.Step(30,  SingleAreaOn()),
                            new Expression.Step(20,  SingleAreaOff()),
                            new Expression.Step(30,  SingleAreaOn()),
                            new Expression.Step(20,  SingleAreaOff()),
                            new Expression.Step(200, SingleAreaOn()),
                            new Expression.Step(400, SingleAreaOff()),
                        }),

                    //  6  Pulse 8 Burst 75 FPM
                    new Expression("Pulse 8 Burst 75 FPM", (UInt16)NamedPatterns.Enum.Pattern_Lightbar_Pulse_8_Burst_75_FPM,
                        0, 1, 0, 100,
                        SingleArea(),
                        new BindingList<Expression.Entry>
                        {
                            new Expression.Step(25,  SingleAreaOn()),
                            new Expression.Step(16,  SingleAreaOff()),
                            new Expression.Step(25,  SingleAreaOn()),
                            new Expression.Step(16,  SingleAreaOff()),
                            new Expression.Step(25,  SingleAreaOn()),
                            new Expression.Step(16,  SingleAreaOff()),
                            new Expression.Step(25,  SingleAreaOn()),
                            new Expression.Step(16,  SingleAreaOff()),
                            new Expression.Step(25,  SingleAreaOn()),
                            new Expression.Step(16,  SingleAreaOff()),
                            new Expression.Step(25,  SingleAreaOn()),
                            new Expression.Step(16,  SingleAreaOff()),
                            new Expression.Step(25,  SingleAreaOn()),
                            new Expression.Step(16,  SingleAreaOff()),
                            new Expression.Step(25,  SingleAreaOn()),
                            new Expression.Step(16,  SingleAreaOff()),
                            new Expression.Step(25,  SingleAreaOn()),
                            new Expression.Step(16,  SingleAreaOff()),
                            new Expression.Step(25,  SingleAreaOn()),
                            new Expression.Step(406, SingleAreaOff()),
                        }),

                    //  7  Reg 65 Single 120 FPM
                    new Expression("Reg 65 Single 120 FPM", (UInt16)NamedPatterns.Enum.Pattern_Lightbar_Reg_65_Single_120_FPM,
                        0, 1, 0, 100,
                        SingleArea(),
                        new BindingList<Expression.Entry>
                        {
                            new Expression.Step(196, SingleAreaOn()),
                            new Expression.Step(300,  SingleAreaOff()),
                        }),

                    //  8  Reg 65 Double 120 FPM
                    new Expression("Reg 65 Double 120 FPM", (UInt16)NamedPatterns.Enum.Pattern_Lightbar_Reg_65_Double_120_FPM, 
                        0, 1, 0, 100,
                        SingleArea(),
                        new BindingList<Expression.Entry>
                        {
                            new Expression.Step(70,  SingleAreaOn()),
                            new Expression.Step(38,  SingleAreaOff()),
                            new Expression.Step(70,  SingleAreaOn()),
                            new Expression.Step(300, SingleAreaOff()),
                        }),

                    //  9  Reg 65 Triple 120 FPM
                    new Expression("Reg 65 Triple 120 FPM", (UInt16)NamedPatterns.Enum.Pattern_Lightbar_Reg_65_Triple_120_FPM,
                        0, 1, 0, 100,
                        SingleArea(),
                        new BindingList<Expression.Entry>
                        {
                            new Expression.Step(47,  SingleAreaOn()),
                            new Expression.Step(23,  SingleAreaOff()),
                            new Expression.Step(47,  SingleAreaOn()),
                            new Expression.Step(23,  SingleAreaOff()),
                            new Expression.Step(47,  SingleAreaOn()),
                            new Expression.Step(295, SingleAreaOff()),
                        }),

                    //  10  Reg 65 Quad 120 FPM
                    new Expression("Reg 65 Quad 120 FPM", (UInt16)NamedPatterns.Enum.Pattern_Lightbar_Reg_65_Quad_120_FPM,
                        0, 1, 0, 100,
                        SingleArea(),
                        new BindingList<Expression.Entry>
                        {
                            new Expression.Step(35,  SingleAreaOn()),
                            new Expression.Step(15,  SingleAreaOff()),
                            new Expression.Step(35,  SingleAreaOn()),
                            new Expression.Step(15,  SingleAreaOff()),
                            new Expression.Step(35,  SingleAreaOn()),
                            new Expression.Step(15,  SingleAreaOff()),
                            new Expression.Step(35,  SingleAreaOn()),
                            new Expression.Step(295, SingleAreaOff()),
                        }),

                    //  11  Chase
                    new Expression("Lower Chase", 1, 0, 1, 2, 100, EighteenAreas(),
                        new BindingList<Expression.Entry>
                        {
                            new Expression.Step(ChaseSpeed, new BindingList<Expression.Token>
                                { new Expression.Token(KeyArea18, 0), new Expression.Token(KeyArea1, 100), }),
                            new Expression.Step(ChaseSpeed, new BindingList<Expression.Token>
                                { new Expression.Token(KeyArea1, 0), new Expression.Token(KeyArea2, 100), }),
                            new Expression.Step(ChaseSpeed, new BindingList<Expression.Token>
                                { new Expression.Token(KeyArea2, 0), new Expression.Token(KeyArea3, 100), }),
                            new Expression.Step(ChaseSpeed, new BindingList<Expression.Token>
                                { new Expression.Token(KeyArea3, 0), new Expression.Token(KeyArea4, 100), }),
                            new Expression.Step(ChaseSpeed, new BindingList<Expression.Token>
                                { new Expression.Token(KeyArea4, 0), new Expression.Token(KeyArea5, 100), }),
                            new Expression.Step(ChaseSpeed, new BindingList<Expression.Token>
                                { new Expression.Token(KeyArea5, 0), new Expression.Token(KeyArea6, 100), }),
                            new Expression.Step(ChaseSpeed, new BindingList<Expression.Token>
                                { new Expression.Token(KeyArea6, 0), new Expression.Token(KeyArea7, 100), }),
                            new Expression.Step(ChaseSpeed, new BindingList<Expression.Token>
                                { new Expression.Token(KeyArea7, 0), new Expression.Token(KeyArea8, 100), }),
                            new Expression.Step(ChaseSpeed, new BindingList<Expression.Token>
                                { new Expression.Token(KeyArea8, 0), new Expression.Token(KeyArea9, 100), }),
                            new Expression.Step(ChaseSpeed, new BindingList<Expression.Token>
                                { new Expression.Token(KeyArea9, 0), new Expression.Token(KeyArea10, 100), }),
                            new Expression.Step(ChaseSpeed, new BindingList<Expression.Token>
                                { new Expression.Token(KeyArea10, 0), new Expression.Token(KeyArea11, 100), }),
                            new Expression.Step(ChaseSpeed, new BindingList<Expression.Token>
                                { new Expression.Token(KeyArea11, 0), new Expression.Token(KeyArea12, 100), }),
                            new Expression.Step(ChaseSpeed, new BindingList<Expression.Token>
                                { new Expression.Token(KeyArea12, 0), new Expression.Token(KeyArea13, 100), }),
                            new Expression.Step(ChaseSpeed, new BindingList<Expression.Token>
                                { new Expression.Token(KeyArea13, 0), new Expression.Token(KeyArea14, 100), }),
                            new Expression.Step(ChaseSpeed, new BindingList<Expression.Token>
                                { new Expression.Token(KeyArea14, 0), new Expression.Token(KeyArea15, 100), }),
                            new Expression.Step(ChaseSpeed, new BindingList<Expression.Token>
                                { new Expression.Token(KeyArea15, 0), new Expression.Token(KeyArea16, 100), }),
                            new Expression.Step(ChaseSpeed, new BindingList<Expression.Token>
                                { new Expression.Token(KeyArea16, 0), new Expression.Token(KeyArea17, 100), }),
                            new Expression.Step(ChaseSpeed, new BindingList<Expression.Token>
                                { new Expression.Token(KeyArea17, 0), new Expression.Token(KeyArea18, 100), }),
                        }),

                    //  12  Arrow Stick
                    new Expression("Arrow Stick", 2, 0, 2, 3, 100, SixAreas(),
                        new BindingList<Expression.Entry>
                        {
                            new Expression.Step(ChaseSpeed, new BindingList<Expression.Token>
                                { new Expression.Token(KeyArea6, 0), new Expression.Token(KeyArea1, 100), }),
                            new Expression.Step(ChaseSpeed, new BindingList<Expression.Token>
                                { new Expression.Token(KeyArea1, 0), new Expression.Token(KeyArea2, 100), }),
                            new Expression.Step(ChaseSpeed, new BindingList<Expression.Token>
                                { new Expression.Token(KeyArea2, 0), new Expression.Token(KeyArea3, 100), }),
                            new Expression.Step(ChaseSpeed, new BindingList<Expression.Token>
                                { new Expression.Token(KeyArea3, 0), new Expression.Token(KeyArea4, 100), }),
                            new Expression.Step(ChaseSpeed, new BindingList<Expression.Token>
                                { new Expression.Token(KeyArea4, 0), new Expression.Token(KeyArea5, 100), }),
                            new Expression.Step(ChaseSpeed, new BindingList<Expression.Token>
                                { new Expression.Token(KeyArea5, 0), new Expression.Token(KeyArea6, 100), }),
                        }),

                });
        }
    }
}
