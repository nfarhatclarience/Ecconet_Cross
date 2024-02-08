<?xml version="1.0"?>
<ExpressionCollection xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" Name="Named Quad Flash Patterns">
  <Expressions>
    <Expression Name="Steady/Cut" RegStandard="None" ExpressionEnum="1026" Repeats="0" InputPriority="1" OututPriority="1" Sequencer="0" Value="40">
      <Areas>
        <Area Name="Steady/Cut Area 1" Key="500" DefaultValue="0">
          <OutputPaths />
        </Area>
      </Areas>
      <Entries>
        <Step Period="100">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
      </Entries>
    </Expression>
    <Expression Name="Quad Flash SS 75 FPM" RegStandard="None" ExpressionEnum="1038" Repeats="0" InputPriority="1" OututPriority="1" Sequencer="0" Value="100">
      <Areas>
        <Area Name="Quad Flash SS 75 FPM Area 1" Key="500" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Quad Flash SS 75 FPM Area 2" Key="501" DefaultValue="0">
          <OutputPaths />
        </Area>
      </Areas>
      <Entries>
        <RepeatSectionStart Repeats="3" />
        <Step Period="40">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="40">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
        <RepeatSectionEnd />
        <Step Period="40">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="120">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
        <RepeatSectionStart Repeats="3" />
        <Step Period="40">
          <Tokens>
            <Token Key="501" Value="100" />
          </Tokens>
        </Step>
        <Step Period="40">
          <Tokens>
            <Token Key="501" Value="0" />
          </Tokens>
        </Step>
        <RepeatSectionEnd />
        <Step Period="40">
          <Tokens>
            <Token Key="501" Value="100" />
          </Tokens>
        </Step>
        <Step Period="120">
          <Tokens>
            <Token Key="501" Value="0" />
          </Tokens>
        </Step>
      </Entries>
    </Expression>
    <Expression Name="Triple Pop 75 FPM" RegStandard="None" ExpressionEnum="1038" Repeats="0" InputPriority="1" OututPriority="1" Sequencer="0" Value="100">
      <Areas>
        <Area Name="Triple Pop 75 FPM Area 1" Key="500" DefaultValue="0">
          <OutputPaths />
        </Area>
      </Areas>
      <Entries>
        <RepeatSectionStart Repeats="2" />
        <Step Period="40">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="120">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
        <RepeatSectionEnd />
        <Step Period="120">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="360">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
      </Entries>
    </Expression>
    <Expression Name="Single 75 FPM" RegStandard="None" ExpressionEnum="1038" Repeats="0" InputPriority="1" OututPriority="1" Sequencer="0" Value="100">
      <Areas>
        <Area Name="Single 75 FPM Area 1" Key="500" DefaultValue="0">
          <OutputPaths />
        </Area>
      </Areas>
      <Entries>
        <Step Period="180">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="620">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
      </Entries>
    </Expression>
    <Expression Name="Reg 65 Double SS 120 FPM" RegStandard="None" ExpressionEnum="1038" Repeats="0" InputPriority="1" OututPriority="1" Sequencer="0" Value="100">
      <Areas>
        <Area Name="Reg 65 Double SS 120 FPM Area 1" Key="500" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Area" Key="501" DefaultValue="0">
          <OutputPaths />
        </Area>
      </Areas>
      <Entries>
        <Step Period="70">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="38">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
        <Step Period="70">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="300">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
        <Step Period="70">
          <Tokens>
            <Token Key="501" Value="100" />
          </Tokens>
        </Step>
        <Step Period="38">
          <Tokens>
            <Token Key="501" Value="0" />
          </Tokens>
        </Step>
        <Step Period="70">
          <Tokens>
            <Token Key="501" Value="100" />
          </Tokens>
        </Step>
        <Step Period="300">
          <Tokens>
            <Token Key="501" Value="0" />
          </Tokens>
        </Step>
      </Entries>
    </Expression>
    <Expression Name="Title 13 Quad 65 FPM" RegStandard="None" ExpressionEnum="1028" Repeats="0" InputPriority="1" OututPriority="1" Sequencer="0" Value="100">
      <Areas>
        <Area Name="Title 13 Quad 65 FPM Area 1" Key="500" DefaultValue="0">
          <OutputPaths />
        </Area>
      </Areas>
      <Entries>
        <Step Period="115">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="40">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
        <Step Period="115">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="40">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
        <Step Period="115">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="40">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
        <Step Period="115">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="340">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
      </Entries>
    </Expression>
    <Expression Name="Title 13 Double 65 FPM" RegStandard="None" ExpressionEnum="1029" Repeats="0" InputPriority="1" OututPriority="1" Sequencer="0" Value="100">
      <Areas>
        <Area Name="Title 13 Double 65 FPM Area 1" Key="500" DefaultValue="0">
          <OutputPaths />
        </Area>
      </Areas>
      <Entries>
        <Step Period="230">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="50">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
        <Step Period="230">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="410">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
      </Entries>
    </Expression>
    <Expression Name="Quint-hold 75 FPM" RegStandard="None" ExpressionEnum="1030" Repeats="0" InputPriority="1" OututPriority="1" Sequencer="0" Value="100">
      <Areas>
        <Area Name="Quint-hold 75 FPM Area 1" Key="500" DefaultValue="0">
          <OutputPaths />
        </Area>
      </Areas>
      <Entries>
        <Step Period="30">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="20">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
        <Step Period="30">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="20">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
        <Step Period="30">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="20">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
        <Step Period="30">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="20">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
        <Step Period="200">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="400">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
      </Entries>
    </Expression>
    <Expression Name="Pulse 8 Burst 75 FPM" RegStandard="None" ExpressionEnum="1031" Repeats="0" InputPriority="1" OututPriority="1" Sequencer="0" Value="100">
      <Areas>
        <Area Name="Pulse 8 Burst 75 FPM Area 1" Key="500" DefaultValue="0">
          <OutputPaths />
        </Area>
      </Areas>
      <Entries>
        <Step Period="25">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="16">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
        <Step Period="25">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="16">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
        <Step Period="25">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="16">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
        <Step Period="25">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="16">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
        <Step Period="25">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="16">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
        <Step Period="25">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="16">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
        <Step Period="25">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="16">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
        <Step Period="25">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="16">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
        <Step Period="25">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="16">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
        <Step Period="25">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="406">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
      </Entries>
    </Expression>
    <Expression Name="Reg 65 Single 120 FPM" RegStandard="None" ExpressionEnum="1032" Repeats="0" InputPriority="1" OututPriority="1" Sequencer="0" Value="100">
      <Areas>
        <Area Name="Reg 65 Single 120 FPM Area 1" Key="500" DefaultValue="0">
          <OutputPaths />
        </Area>
      </Areas>
      <Entries>
        <Step Period="196">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="300">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
      </Entries>
    </Expression>
    <Expression Name="Reg 65 Double 120 FPM" RegStandard="None" ExpressionEnum="1033" Repeats="0" InputPriority="1" OututPriority="1" Sequencer="0" Value="100">
      <Areas>
        <Area Name="Reg 65 Double 120 FPM Area 1" Key="500" DefaultValue="0">
          <OutputPaths />
        </Area>
      </Areas>
      <Entries>
        <Step Period="70">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="38">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
        <Step Period="70">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="300">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
      </Entries>
    </Expression>
    <Expression Name="Reg 65 Triple 120 FPM" RegStandard="None" ExpressionEnum="1034" Repeats="0" InputPriority="1" OututPriority="1" Sequencer="0" Value="100">
      <Areas>
        <Area Name="Reg 65 Triple 120 FPM Area 1" Key="500" DefaultValue="0">
          <OutputPaths />
        </Area>
      </Areas>
      <Entries>
        <Step Period="47">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="23">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
        <Step Period="47">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="23">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
        <Step Period="47">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="295">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
      </Entries>
    </Expression>
    <Expression Name="Reg 65 Quad 120 FPM" RegStandard="None" ExpressionEnum="1035" Repeats="0" InputPriority="1" OututPriority="1" Sequencer="0" Value="100">
      <Areas>
        <Area Name="Reg 65 Quad 120 FPM Area 1" Key="500" DefaultValue="0">
          <OutputPaths />
        </Area>
      </Areas>
      <Entries>
        <Step Period="35">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="15">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
        <Step Period="35">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="15">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
        <Step Period="35">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="15">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
        <Step Period="35">
          <Tokens>
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="295">
          <Tokens>
            <Token Key="500" Value="0" />
          </Tokens>
        </Step>
      </Entries>
    </Expression>
    <Expression Name="Arrow 18 200" RegStandard="None" ExpressionEnum="1" Repeats="0" InputPriority="1" OututPriority="1" Sequencer="0" Value="100">
      <Areas>
        <Area Name="Arrow 18 200 Area 1" Key="500" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 18 200 Area 2" Key="501" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 18 200 Area 3" Key="502" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 18 200 Area 4" Key="503" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 18 200 Area 5" Key="504" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 18 200 Area 6" Key="505" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 18 200 Area 7" Key="506" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 18 200 Area 8" Key="507" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 18 200 Area 9" Key="508" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 18 200 Area 10" Key="509" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 18 200 Area 11" Key="510" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 18 200 Area 12" Key="511" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 18 200 Area 13" Key="512" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 18 200 Area 14" Key="513" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 18 200 Area 15" Key="514" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 18 200 Area 16" Key="515" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 18 200 Area 17" Key="516" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 18 200 Area 18" Key="517" DefaultValue="0">
          <OutputPaths />
        </Area>
      </Areas>
      <Entries>
        <Step Period="200">
          <Tokens>
            <Token Key="517" Value="0" />
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="200">
          <Tokens>
            <Token Key="500" Value="0" />
            <Token Key="501" Value="100" />
          </Tokens>
        </Step>
        <Step Period="200">
          <Tokens>
            <Token Key="501" Value="0" />
            <Token Key="502" Value="100" />
          </Tokens>
        </Step>
        <Step Period="200">
          <Tokens>
            <Token Key="502" Value="0" />
            <Token Key="503" Value="100" />
          </Tokens>
        </Step>
        <Step Period="200">
          <Tokens>
            <Token Key="503" Value="0" />
            <Token Key="504" Value="100" />
          </Tokens>
        </Step>
        <Step Period="200">
          <Tokens>
            <Token Key="504" Value="0" />
            <Token Key="505" Value="100" />
          </Tokens>
        </Step>
        <Step Period="200">
          <Tokens>
            <Token Key="505" Value="0" />
            <Token Key="506" Value="100" />
          </Tokens>
        </Step>
        <Step Period="200">
          <Tokens>
            <Token Key="506" Value="0" />
            <Token Key="507" Value="100" />
          </Tokens>
        </Step>
        <Step Period="200">
          <Tokens>
            <Token Key="507" Value="0" />
            <Token Key="508" Value="100" />
          </Tokens>
        </Step>
        <Step Period="200">
          <Tokens>
            <Token Key="508" Value="0" />
            <Token Key="509" Value="100" />
          </Tokens>
        </Step>
        <Step Period="200">
          <Tokens>
            <Token Key="509" Value="0" />
            <Token Key="510" Value="100" />
          </Tokens>
        </Step>
        <Step Period="200">
          <Tokens>
            <Token Key="510" Value="0" />
            <Token Key="511" Value="100" />
          </Tokens>
        </Step>
        <Step Period="200">
          <Tokens>
            <Token Key="511" Value="0" />
            <Token Key="512" Value="100" />
          </Tokens>
        </Step>
        <Step Period="200">
          <Tokens>
            <Token Key="512" Value="0" />
            <Token Key="513" Value="100" />
          </Tokens>
        </Step>
        <Step Period="200">
          <Tokens>
            <Token Key="513" Value="0" />
            <Token Key="514" Value="100" />
          </Tokens>
        </Step>
        <Step Period="200">
          <Tokens>
            <Token Key="514" Value="0" />
            <Token Key="515" Value="100" />
          </Tokens>
        </Step>
        <Step Period="200">
          <Tokens>
            <Token Key="515" Value="0" />
            <Token Key="516" Value="100" />
          </Tokens>
        </Step>
        <Step Period="200">
          <Tokens>
            <Token Key="516" Value="0" />
            <Token Key="517" Value="100" />
          </Tokens>
        </Step>
      </Entries>
    </Expression>
    <Expression Name="Arrow 6 200" RegStandard="None" ExpressionEnum="2" Repeats="0" InputPriority="1" OututPriority="1" Sequencer="0" Value="100">
      <Areas>
        <Area Name="Arrow 6 200 Area 1" Key="500" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 6 200 Area 2" Key="501" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 6 200 Area 3" Key="502" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 6 200 Area 4" Key="503" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 6 200 Area 5" Key="504" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 6 200 Area 6" Key="505" DefaultValue="0">
          <OutputPaths />
        </Area>
      </Areas>
      <Entries>
        <Step Period="200">
          <Tokens>
            <Token Key="505" Value="0" />
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="200">
          <Tokens>
            <Token Key="500" Value="0" />
            <Token Key="501" Value="100" />
          </Tokens>
        </Step>
        <Step Period="200">
          <Tokens>
            <Token Key="501" Value="0" />
            <Token Key="502" Value="100" />
          </Tokens>
        </Step>
        <Step Period="200">
          <Tokens>
            <Token Key="502" Value="0" />
            <Token Key="503" Value="100" />
          </Tokens>
        </Step>
        <Step Period="200">
          <Tokens>
            <Token Key="503" Value="0" />
            <Token Key="504" Value="100" />
          </Tokens>
        </Step>
        <Step Period="200">
          <Tokens>
            <Token Key="504" Value="0" />
            <Token Key="505" Value="100" />
          </Tokens>
        </Step>
      </Entries>
    </Expression>
    <Expression Name="Arrow 8 200" RegStandard="None" ExpressionEnum="3" Repeats="0" InputPriority="1" OututPriority="1" Sequencer="0" Value="100">
      <Areas>
        <Area Name="Arrow 8 200 Area 1" Key="500" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 8 200 Area 2" Key="501" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 8 200 Area 3" Key="502" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 8 200 Area 4" Key="503" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 8 200 Area 5" Key="504" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 8 200 Area 6" Key="505" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 8 200 Area 7" Key="506" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 8 200 Area 8" Key="507" DefaultValue="0">
          <OutputPaths />
        </Area>
      </Areas>
      <Entries>
        <Step Period="200">
          <Tokens>
            <Token Key="507" Value="0" />
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="200">
          <Tokens>
            <Token Key="500" Value="0" />
            <Token Key="501" Value="100" />
          </Tokens>
        </Step>
        <Step Period="200">
          <Tokens>
            <Token Key="501" Value="0" />
            <Token Key="502" Value="100" />
          </Tokens>
        </Step>
        <Step Period="200">
          <Tokens>
            <Token Key="502" Value="0" />
            <Token Key="503" Value="100" />
          </Tokens>
        </Step>
        <Step Period="200">
          <Tokens>
            <Token Key="503" Value="0" />
            <Token Key="504" Value="100" />
          </Tokens>
        </Step>
        <Step Period="200">
          <Tokens>
            <Token Key="504" Value="0" />
            <Token Key="505" Value="100" />
          </Tokens>
        </Step>
        <Step Period="200">
          <Tokens>
            <Token Key="505" Value="0" />
            <Token Key="506" Value="100" />
          </Tokens>
        </Step>
        <Step Period="200">
          <Tokens>
            <Token Key="506" Value="0" />
            <Token Key="507" Value="100" />
          </Tokens>
        </Step>
      </Entries>
    </Expression>
    <Expression Name="Arrow 4 200" RegStandard="None" ExpressionEnum="4" Repeats="0" InputPriority="1" OututPriority="1" Sequencer="0" Value="100">
      <Areas>
        <Area Name="Arrow 4 200 Area 1" Key="500" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 4 200 Area 2" Key="501" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 4 200 Area 3" Key="502" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 4 200 Area 4" Key="503" DefaultValue="0">
          <OutputPaths />
        </Area>
      </Areas>
      <Entries>
        <Step Period="200">
          <Tokens>
            <Token Key="503" Value="0" />
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="200">
          <Tokens>
            <Token Key="500" Value="0" />
            <Token Key="501" Value="100" />
          </Tokens>
        </Step>
        <Step Period="200">
          <Tokens>
            <Token Key="501" Value="0" />
            <Token Key="502" Value="100" />
          </Tokens>
        </Step>
        <Step Period="200">
          <Tokens>
            <Token Key="502" Value="0" />
            <Token Key="503" Value="100" />
          </Tokens>
        </Step>
      </Entries>
    </Expression>
    <Expression Name="Arrow 8 50" RegStandard="None" ExpressionEnum="5" Repeats="0" InputPriority="1" OututPriority="1" Sequencer="0" Value="100">
      <Areas>
        <Area Name="Arrow 8 50 Area 1" Key="500" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 8 50 Area 2" Key="501" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 8 50 Area 3" Key="502" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 8 50 Area 4" Key="503" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 8 50 Area 5" Key="504" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 8 50 Area 6" Key="505" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 8 50 Area 7" Key="506" DefaultValue="0">
          <OutputPaths />
        </Area>
        <Area Name="Arrow 8 50 Area 8" Key="507" DefaultValue="0">
          <OutputPaths />
        </Area>
      </Areas>
      <Entries>
        <Step Period="50">
          <Tokens>
            <Token Key="507" Value="0" />
            <Token Key="500" Value="100" />
          </Tokens>
        </Step>
        <Step Period="50">
          <Tokens>
            <Token Key="500" Value="0" />
            <Token Key="501" Value="100" />
          </Tokens>
        </Step>
        <Step Period="50">
          <Tokens>
            <Token Key="501" Value="0" />
            <Token Key="502" Value="100" />
          </Tokens>
        </Step>
        <Step Period="50">
          <Tokens>
            <Token Key="502" Value="0" />
            <Token Key="503" Value="100" />
          </Tokens>
        </Step>
        <Step Period="50">
          <Tokens>
            <Token Key="503" Value="0" />
            <Token Key="504" Value="100" />
          </Tokens>
        </Step>
        <Step Period="50">
          <Tokens>
            <Token Key="504" Value="0" />
            <Token Key="505" Value="100" />
          </Tokens>
        </Step>
        <Step Period="50">
          <Tokens>
            <Token Key="505" Value="0" />
            <Token Key="506" Value="100" />
          </Tokens>
        </Step>
        <Step Period="50">
          <Tokens>
            <Token Key="506" Value="0" />
            <Token Key="507" Value="100" />
          </Tokens>
        </Step>
      </Entries>
    </Expression>
  </Expressions>
</ExpressionCollection>