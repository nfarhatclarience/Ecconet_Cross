<?xml version="1.0"?>
<ExpressionCollection xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <Expressions>
    <Expression Name="Quad Flash SS 75 FPM" RegStandard="None" ExpressionEnum="3" Repeats="0" InputPriority="7" OutputPriority="7" Sequencer="0" Value="100">
      <Areas>
        <Area Name="Quad Flash SS 75 FPM Area 1" Key="500" DefaultValue="0">
          <Outputs>
            <Output Path="532" Value="100" />
            <Output Path="531" Value="100" />
            <Output Path="530" Value="100" />
            <Output Path="529" Value="100" />
            <Output Path="54" Value="100" />
            <Output Path="53" Value="100" />
            <Output Path="52" Value="100" />
          </Outputs>
        </Area>
        <Area Name="Quad Flash SS 75 FPM Area 2" Key="501" DefaultValue="0">
          <Outputs>
            <Output Path="520" Value="100" />
            <Output Path="519" Value="100" />
            <Output Path="518" Value="100" />
            <Output Path="513" Value="100" />
            <Output Path="514" Value="100" />
            <Output Path="515" Value="100" />
          </Outputs>
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
    <Expression Name="Triple Pop 75 FPM" RegStandard="None" ExpressionEnum="4" Repeats="0" InputPriority="6" OutputPriority="6" Sequencer="0" Value="100">
      <Areas>
        <Area Name="Triple Pop 75 FPM Area 1" Key="500" DefaultValue="0">
          <Outputs>
            <Output Path="530" Value="100" />
            <Output Path="529" Value="100" />
            <Output Path="520" Value="100" />
            <Output Path="519" Value="100" />
          </Outputs>
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
    <Expression Name="Arrow 8 50" RegStandard="None" ExpressionEnum="6" Repeats="0" InputPriority="4" OutputPriority="4" Sequencer="3" Value="100">
      <Areas>
        <Area Name="Arrow 8 50 Area 1" Key="500" DefaultValue="0">
          <Outputs>
            <Output Path="532" Value="100" />
          </Outputs>
        </Area>
        <Area Name="Arrow 8 50 Area 2" Key="501" DefaultValue="0">
          <Outputs>
            <Output Path="531" Value="100" />
          </Outputs>
        </Area>
        <Area Name="Arrow 8 50 Area 3" Key="502" DefaultValue="0">
          <Outputs>
            <Output Path="530" Value="100" />
          </Outputs>
        </Area>
        <Area Name="Arrow 8 50 Area 4" Key="503" DefaultValue="0">
          <Outputs>
            <Output Path="529" Value="100" />
          </Outputs>
        </Area>
        <Area Name="Arrow 8 50 Area 5" Key="504" DefaultValue="0">
          <Outputs>
            <Output Path="520" Value="100" />
          </Outputs>
        </Area>
        <Area Name="Arrow 8 50 Area 6" Key="505" DefaultValue="0">
          <Outputs>
            <Output Path="519" Value="100" />
          </Outputs>
        </Area>
        <Area Name="Arrow 8 50 Area 7" Key="506" DefaultValue="0">
          <Outputs>
            <Output Path="518" Value="100" />
          </Outputs>
        </Area>
        <Area Name="Arrow 8 50 Area 8" Key="507" DefaultValue="0">
          <Outputs>
            <Output Path="517" Value="100" />
          </Outputs>
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
    <Expression Name="Arrow Right 8 B1-B8" RegStandard="None" ExpressionEnum="8" Repeats="0" InputPriority="2" OutputPriority="2" Sequencer="3" Value="100">
      <Areas>
        <Area Name="Arrow 8 200 Area 1" Key="500" DefaultValue="0">
          <Outputs>
            <Output Path="532" Value="100" />
          </Outputs>
        </Area>
        <Area Name="Arrow 8 200 Area 2" Key="501" DefaultValue="0">
          <Outputs>
            <Output Path="531" Value="100" />
          </Outputs>
        </Area>
        <Area Name="Arrow 8 200 Area 3" Key="502" DefaultValue="0">
          <Outputs>
            <Output Path="530" Value="100" />
          </Outputs>
        </Area>
        <Area Name="Arrow 8 200 Area 4" Key="503" DefaultValue="0">
          <Outputs>
            <Output Path="529" Value="100" />
          </Outputs>
        </Area>
        <Area Name="Arrow 8 200 Area 5" Key="504" DefaultValue="0">
          <Outputs>
            <Output Path="520" Value="100" />
          </Outputs>
        </Area>
        <Area Name="Arrow 8 200 Area 6" Key="505" DefaultValue="0">
          <Outputs>
            <Output Path="519" Value="100" />
          </Outputs>
        </Area>
        <Area Name="Arrow 8 200 Area 7" Key="506" DefaultValue="0">
          <Outputs>
            <Output Path="518" Value="100" />
          </Outputs>
        </Area>
        <Area Name="Arrow 8 200 Area 8" Key="507" DefaultValue="0">
          <Outputs>
            <Output Path="517" Value="100" />
          </Outputs>
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
    <Expression Name="Steady Override A1 &amp; B1" RegStandard="None" ExpressionEnum="10" Repeats="0" InputPriority="1" OutputPriority="1" Sequencer="0" Value="30">
      <Areas>
        <Area Name="Steady 40% Area 1" Key="500" DefaultValue="0">
          <Outputs>
            <Output Path="532" Value="100" />
            <Output Path="531" Value="100" />
            <Output Path="530" Value="100" />
            <Output Path="529" Value="100" />
            <Output Path="520" Value="100" />
            <Output Path="519" Value="100" />
            <Output Path="518" Value="100" />
            <Output Path="517" Value="100" />
            <Output Path="52" Value="100" />
            <Output Path="53" Value="100" />
            <Output Path="54" Value="100" />
            <Output Path="513" Value="100" />
            <Output Path="514" Value="100" />
            <Output Path="515" Value="100" />
          </Outputs>
        </Area>
      </Areas>
      <Entries>
        <Step Period="100">
          <Tokens>
            <Token Key="500" Value="50" />
          </Tokens>
        </Step>
      </Entries>
    </Expression>
    <Expression Name="Cut A2 &amp; B2" RegStandard="None" ExpressionEnum="11" Repeats="0" InputPriority="1" OutputPriority="1" Sequencer="0" Value="40">
      <Areas>
        <Area Name="Steady 40% Area 1" Key="500" DefaultValue="0">
          <Outputs>
            <Output Path="532" Value="100" />
            <Output Path="531" Value="100" />
            <Output Path="515" Value="100" />
            <Output Path="530" Value="100" />
            <Output Path="514" Value="100" />
          </Outputs>
        </Area>
      </Areas>
      <Entries>
        <Step Period="100">
          <Tokens>
            <Token Key="500" Value="50" />
          </Tokens>
        </Step>
      </Entries>
    </Expression>
  </Expressions>
</ExpressionCollection>