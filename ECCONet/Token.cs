/**
  ******************************************************************************
  * @file    	Token.cs
  * @copyright  © 2017 ECCO Group.  All rights reserved.
  * @author  	M. Latham, Liquid Logic, LLC
  * @version 	1.0.0
  * @date    	April 2017
  * @brief   	ESG Matrix token object, regions and keys.
  ******************************************************************************
  * @attention
  *
  * Unless required by applicable law or agreed to in writing, software created
  * by Liquid Logic, LLC is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES
  *	OR CONDITIONS OF ANY KIND, either express or implied.
  *
  ******************************************************************************
  */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCONet
{
    public class Token
    {
        //  key
        public Keys key;

        //  value
        public Int32 value;

        //  address
        public byte address;

        //  event index, used only for dev tool observation
        public byte eventIndex;


        public const UInt32 TOKEN_KEY_SIZE = 2;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Token()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="key">The enumerated key.</param>
        /// <param name="value">The value.</param>
        /// <param name="address">The address.</param>
        public Token(Keys key, Int32 value, byte address)
        {
            this.key = key;
            this.value = value;
            this.address = address;
            this.eventIndex = 0;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="key">The enumerated key.</param>
        /// <param name="value">The value.</param>
        /// <param name="address">The address.</param>
        /// <param name="eventIndex">The event index, used for dev tool observation.</param>
        public Token(Keys key, Int32 value, byte address, byte eventIndex)
        {
            this.key = key;
            this.value = value;
            this.address = address;
            this.eventIndex = eventIndex;
        }

        //	This region is for private local variables and the keys may
        //	be used for any purpose.
        //
        //	Local variables are signed 32-bit values.
        //
        public const UInt16 Region_Base__Local_Variables = 1;
        public const UInt16 Region_Size__Local_Variables = 199;

        //	The defines are for reference only. They are used for compressing
        //	local variables in an AnyToken bin pattern file, and decompressed
        //	values are 32 bits.
        public const UInt16 Value_Bytes__Local_Variables = 4;
        public const UInt16 Subreg_Base__Local_One_Byte = 1;
        public const UInt16 Subreg_Size__Local_One_Byte = 119;
        public const UInt16 Value_Bytes__Local_One_Byte = 1;
        public const UInt16 Subreg_Base__Local_Two_Byte = 120;
        public const UInt16 Subreg_Size__Local_Two_Byte = 50;
        public const UInt16 Value_Bytes__Local_Two_Byte = 2;
        public const UInt16 Subreg_Base__Local_Four_Byte = 170;
        public const UInt16 Subreg_Size__Local_Four_Byte = 20;
        public const UInt16 Value_Bytes__Local_Four_Byte = 4;
        public const UInt16 Subreg_Base__Local_Zero_Byte = 190;
        public const UInt16 Subreg_Size__Local_Zero_Byte = 10;
        public const UInt16 Value_Bytes__Local_Zero_Byte = 0;


        //	This region is for public indexed arrays of non-descript 8-bit inputs.
        //	An example would be a graphics-based input device with arrays of user
        //	controls.
        //
        //	Device firmware maps the keys to specific inputs.
        //	Although 8 bits are specified, the input is typically boolean.
        //
        public const UInt16 Region_Base__Indexed_One_Byte_Inputs = 200;
        public const UInt16 Region_Size__Indexed_One_Byte_Inputs = 300;
        public const UInt16 Value_Bytes__Indexed_One_Byte_Input = 1;


        //	This region is for public indexed arrays of non-descript 8-bit outputs.
        //	An example would be an array of light heads used for patterns.
        //
        //	Device firmware maps the keys to specific outputs.
        //
        //	These outputs must be responsive to commands tokens.  Smart light heads
        //	that have a component failure warning should issue output status tokens
        //	only when a failure occurs, and no more often than once per second.
        //
        public const UInt16 Region_Base__Indexed_One_Byte_Outputs = 500;
        public const UInt16 Region_Size__Indexed_One_Byte_Outputs = 500;
        public const UInt16 Value_Bytes__Indexed_One_Byte_Output = 1;


        //	This region is for public 8-bit named items.  The naming
        //	comes from international standards committees or ESG internal.
        //
        //	For lights and speakers, the associated value is the relative
        //	light intensity or acoustic intensity 0~100.
        //
        //	Organizing into subregions below facilitates tests for certain
        //	categories, and provides a clearer picture to third-party OEMs
        //	who may use such keys for direct asset control.
        //
        public const UInt16 Region_Base__Named_One_Byte = 1000;
        public const UInt16 Region_Size__Named_One_Byte = 4000;
        public const UInt16 Value_Bytes__Named_One_Byte = 1;
        public const UInt16 Subreg_Base__Named_Lights = 1000;
        public const UInt16 Subreg_Size__Named_Lights = 400;
        public const UInt16 Subreg_Base__Named_Sounds = 1400;
        public const UInt16 Subreg_Size__Named_Sounds = 400;
        public const UInt16 Subreg_Base__Named_Messages = 1800;
        public const UInt16 Subreg_Size__Named_Messages = 200;
        public const UInt16 Subreg_Base__Named_OBD2_One_Byte = 2000;
        public const UInt16 Subreg_Size__Named_OBD2_One_Byte = 1000;
        public const UInt16 Region_Base__Named_Misc_One_Byte = 3000;
        public const UInt16 Region_Size__Named_Misc_One_Byte = 2000;


        //	This region is for public 16-bit named items.  The naming
        //	comes from international standards committees or ESG internal.
        //
        public const UInt16 Region_Base__Named_Two_Byte = 5000;
        public const UInt16 Region_Size__Named_Two_Byte = 2000;
        public const UInt16 Value_Bytes__Named_Two_Byte = 2;
        public const UInt16 Subreg_Base__IndexedLedLifetime = 6800;
        public const UInt16 Subreg_Size__IndexedLedLifetime = 200;


        //	This region is for public signed 32-bit named items.  The naming
        //	comes from international standards committees or ESG internal.
        //
        public const UInt16 Region_Base__Named_Four_Byte = 7000;
        public const UInt16 Region_Size__Named_Four_Byte = 1000;
        public const UInt16 Value_Bytes__Named_Four_Byte = 4;
        public const UInt16 Subreg_Base__Indexed_Sequencer_Four_Byte = 7900;
        public const UInt16 Subreg_Size__Indexed_Sequencer_Four_Byte = 50;
        public const UInt16 Subreg_Base__Named_Four_Byte_C3Net = 7950;
        public const UInt16 Subreg_Size__Named_Four_Byte_C3Net = 50;


        //	This region is for public named zero-byte items.  The naming
        //	comes from international standards committees or ESG internal.
        //
        public const UInt16 Region_Base__Named_Zero_Byte = 8000;
        public const UInt16 Region_Size__Named_Zero_Byte = 150;
        public const UInt16 Value_Bytes__Named_Zero_Byte = 0;


        //	This region is for public indexed sequencers.
        //	(intensity << 16) | pattern_enumeration
        //
        public const UInt16 Region_Base__Indexed_Sequencer_Three_Byte = 8150;
        public const UInt16 Region_Size__Indexed_Sequencer_Three_Byte = 10;
        public const UInt16 Value_Bytes__Indexed_Sequencer_Three_Byte = 3;


        //	This region is for public named FTP requests.
        //
        public const UInt16 Region_Base__FTP_Requests = 8160;
        public const UInt16 Region_Size__FTP_Requests = 10;

        //	This region is for public named FTP responses.
        //
        public const UInt16 Region_Base__FTP_Responses   = 8170;
        public const UInt16 Region_Size__FTP_Response    = 22;


        /// <summary>
        /// Enumerated token keys.
        /// Keys are tagged as input status, output status or command with the prefixes below.
        /// </summary>
        public enum Keys
        {
            //	reserved null key
            KeyNull,

            //	named lights																	//	Value format
            //																					//	===================================================
            KeyLight_Stop = Subreg_Base__Named_Lights,                                          //	As input status: Boolean, 1=on;  As command: intensity 0~100
            KeyLight_Tail,                                                                      //	As input status: Boolean, 1=on;  As command: intensity 0~100
            KeyLight_LeftTurn,                                                                  //	As input status: Boolean, 1=on;  As command: intensity 0~100
            KeyLight_RightTurn,                                                                 //	As input status: Boolean, 1=on;  As command: intensity 0~100
            KeyLight_Takedown,                                                                  //	As input status: Boolean, 1=on;  As command: intensity 0~100
            KeyLight_Worklight,                                                                 //	As input status: Boolean, 1=on;  As command: intensity 0~100
            KeyLight_AlleyLeft,                                                                 //	As input status: Boolean, 1=on;  As command: intensity 0~100
            KeyLight_AlleyRight,                                                                //	As input status: Boolean, 1=on;  As command: intensity 0~100
            KeyLight_WorklightLeft,                                                             //	As input status: Boolean, 1=on;  As command: intensity 0~100
            KeyLight_WorklightRight,                                                            //  As input status: Boolean, 1=on;  As command: intensity 0~100
            KeyLight_TakedownLeft,                                                              //  As input status: Boolean, 1=on;  As command: intensity 0~100
            KeyLight_TakedownRight,                                                             //  As input status: Boolean, 1=on;  As command: intensity 0~100
            KeyLight_SteadyFrontLeft,                                                           //  As input status: Boolean, 1=on;  As command: intensity 0~100
            KeyLight_SteadyFrontRight,                                                          //  As input status: Boolean, 1=on;  As command: intensity 0~100
            KeyLight_ButtonBlast1,                                                              //  As input status: Boolean, 1=on;  As command: intensity 0~100
            KeyLight_ButtonBlast2,                                                              //  As input status: Boolean, 1=on;  As command: intensity 0~100

            KeyVehicleHorn = (Subreg_Base__Named_OBD2_One_Byte - 2),
            KeyVehicleHornOEM = (Subreg_Base__Named_OBD2_One_Byte - 1),

            //	named OBD-II one-byte										                    //	Value format
            //																                    //	===================================================
            KeyACClutchButton = Subreg_Base__Named_OBD2_One_Byte,                               //	Boolean, 1=on
            KeyDriverSideFrontDoorOpen,                                                         //	Boolean, 1=open
            KeyDriverSideRearDoorOpen,                                                          //	Boolean, 1=open
            KeyPassengerSideFrontDoorOpen,                                                      //	Boolean, 1=open
            KeyPassSideRearDoorOpen,                                                            //	Boolean, 1=open
            KeyRearHatchOpen,                                                                   //	Boolean, 1=open
            KeyRearWindowPosition,                                                              //	Boolean, 1=open
            KeyDoorsLocked,                                                                     //	Boolean, 1=locked (all doors)
            KeyHeadlightLowBeam,                                                                //	Boolean, 1=beam on
            KeyHeadlightHighBeam,                                                               //	Boolean, 1=beam on
            KeyLeftTurnSignal,                                                                  //	Boolean, 1=light on (as opposed to feature)
            KeyRightTurnSignal,                                                                 //	Boolean, 1=light on (as opposed to feature)
            KeyHazards,                                                                         //	Boolean, 1=lights on (as opposed to feature)
            KeyMarkerLights,                                                                    //	Boolean, 1=on
            KeyParkBrake,                                                                       //	Boolean, 1=brake applied
            KeyServiceBrake,                                                                    //	Boolean, 1=brake pedal engaged
            KeyDriverSeatbeltFastened,                                                          //	Boolean, 1=fastened
            KeyPassengerSeatbeltFastened,                                                       //	Boolean, 1=fastened
            KeyRearSeatbeltsFastened,                                                           //	Boolean, 1=fastened
            KeyKeyPosition,                                                                     //	8-bit flags { Off=0,Acc=1,Run=2,Start=4 }
            KeyTransmissionPosition,                                                            //	8-bit enum { P=0,N=1,D=2,R=3 }
            KeyThrottlePosition,                                                                // 	8-bit 0~100, 0=least fuel metering
            KeyVehicleSpeed,                                                                    //	8-bit 0-255 km/h
            KeyFuelLevel,                                                                       //	8-bit 0~100, 0=empty
            KeyBatteryVoltage,                                                                  //	8-bit decivolt
            KeyVehicleAcceleration,											                    //	8-bit signed +/- 2G

            //	named one-byte												                    //	Value format
            //																                    //	===================================================
            KeyRequestAddress = Region_Base__Named_Misc_One_Byte,                               //	8-bit requesting address 1~120 during bus enum
            KeyResponseAddressInUse,                                                            //	8-bit address 1~120 in use during bus enum
            KeySystemPowerState,                                                                //	8-bit enum { Stby=0, Active=1 }
            KeyTokenSequencerIntensity,                                                         //	8-bit 0~100 (%) using local sequencer address
            KeyNextExpressionFront,                                                             //	Boolean, for field programming, rising edge = next available
            KeyNextExpressionRear,                                                              //	Boolean, for field programming, rising edge = next available
            KeyOutputAuxiliary,                                                                 //	Boolean, 1=on
            KeyModeCruise,                                                                      //	Boolean, 1=on
            KeyModeNight,                                                                       //	Boolean, 1=on
            KeyExpressionPresetLightBarFront,                                                   //	8-bit bit flags, preset P1=0x01, P2=0x02, P3=0x04, etc.
            KeyExpressionPresetLightBarRear,                                                    //	8-bit bit flags, preset P1=0x01, P2=0x02, P3=0x04, etc.
            KeyExpressionPresetDirectorFront,                                                   //	8-bit bit flags, preset P1=0x01, P2=0x02, P3=0x04, etc.
            KeyExpressionPresetDirectorRear,                                                    //	8-bit bit flags, preset P1=0x01, P2=0x02, P3=0x04, etc.
            KeyIndexedPatternPresetSound,                                                       //	8-bit user preset selection
            KeyRotatingBeaconControl,                                                           //	8-bit enum for rotating beacon control
            KeyNodeType,                                                                        //	8-bit node type, 0 = unknown
            KeyAlertLevel,                                                                      //	8-bit bit flags L1 = 0x01, L2 = 0x02, L3 = 0x04
            KeyVehicleAlarm,                                                                    //	Boolean, special functions that can start while vehicle is off
            KeyUserProfile,                                                                     //	8-bit user profile enum
            KeyLeftCut,                                                                         //	Boolean, 1=active
            KeyRightCut,                                                                        //	Boolean, 1=active
            KeyFrontCut,                                                                        //	Boolean, 1=active
            KeyRearCut,                                                                         //	Boolean, 1=active
            KeyDirectorNumLights,                                                               //	8-bit, indicates number of director LEDs used
            KeyNextDirectorNumLights,                                                           //	Boolean, for field programming, rising edge = next available
            KeyDirectorLocation,                                                                //	8-bit enum, 0=rear, 1=front, 2=both
            KeyNextDirectorLocation,                                                            //	Boolean, for field programming, rising edge = next available
            KeyNextPrimaryExpression,                                                           //	Boolean, for field programming, rising edge = next available
            KeyParkKill,                                                                        //  ??  PLEASE FILL IN !!
            KeyPhotoSensor,                                                                     //  ??  PLEASE FILL IN !!
            KeyDirectionalDirection,                                                            //  0 = No directional pattern running, 1=left, 2=right, 3=center, 4=flash
            KeyInterlock,
            KeyResponseUpdateHardwareRevision,                                                  //  1 = success, 0 = failure
            KeyRequestNodeIdentify,                                                             //  1 = this device is being identified, 0 = this device is done identifying (action during identification is device-specific)
            KeyTestFixtureCmd,                                                                  //  Application-specific token for optional test fixture use
            KeyResponseLockAddress,                                                             //  0 = successfully locked address, Flash Drive Error Code (negative number) if writing address.can failed
            KeyAlertLevelWithoutDrop,

            //	named two-byte												                    //	Value format
            //																                    //	===================================================
            KeyTokenSequencerPattern = Region_Base__Named_Two_Byte,                             //	16-bit pattern enum using sequencer address (stop=0)
            KeyTokenSequencerSync,                                                              //	16-bit sync pattern enum
            KeyResponseAppFirmwareCrc,                                                          //	16-bit ECCONet 3.0 CRC
            KeyResponseBootloaderFirmwareCrc,                                                   //	16-bit ECCONet 3.0 CRC
            KeyEngineRPM,                                                                       //	16-bit OBD-II integer RPM
            KeySafetyDirFrontPattern,                                                           //	16-bit pattern enum (stop=0) (for front SD)
            KeySafetyDirRearPattern,                                                            //	16-bit pattern enum (stop=0) (for rear SD)
            KeyStepMethodDictionaryKey,                                                         //	16-bit light engine dictionary key
            KeyDeviceFault,                                                                     //	(4-bit flags<<12) | (12-bit faultCode) (faultCode 0=none)
            KeyJboxState,                                                                       //	16-bit JBox port bit state
            KeyReportedLightbarEnumSequencer0,
            KeyReportedLightbarEnumSequencer1,
            KeyReportedLightbarEnumSequencer2,
            KeyReportedLightbarEnumSequencer3,
            KeyReportedLightbarEnumSequencer4,
            KeyReportedLightbarEnumSequencer5,
            KeyReportedLightbarEnumSequencer6,
            KeyReportedLightbarEnumSequencer7,
            KeyReportedLightbarEnumSequencer8,
            KeyReportedLightbarEnumSequencer9,
            KeyIndexedBarOutput,                                                                //  (1-bit color<<15) | (7-bit index<<8) | intensity 0~100
            KeyIndexedLedLifetimeReset,                                                         //  16-bit LED lifetime zero-based key  
            KeyFrontLightSensorLux,                                                             //  16-bit front light sensor lux reading
            KeyRearLightSensorLux,                                                              //  16-bit rear light sensor lux reading

            //	named four-byte												                    //	Value format
            //																                    //	===================================================
            KeyIndexedTokenSequencerWithPattern = Region_Base__Named_Four_Byte,                 //	(exprEnum<<16)|(intensity<<8)|sequencerIndex (exprEnum 0=stop)
            KeyRequestSystemReboot,                                                             //	32-bit, must be 0x4C7E146F
            KeyRequestInvokeBootloader,                                                         //	32-bit, must be 0x5633870B
            KeyRequestEraseAppFirmware,                                                         //	32-bit, must be 0x6A783B52
            KeyRequestEraseAllFirmware,                                                         //	32-bit, must be 0xB8E0123C
            KeyRequestAllowStatus,                                                              //	32-bit, must be 0x320CCAEC
            KeyRequestSuppressStatus,                                                           //	32-bit, must be 0xA7CA28EE
            KeyTokenSequencerSyncRange,                                                         //	2 x 16-bit, must use local sequencer address																	
            KeySoundEnumWithIndexedAmp,                                                         //	(soundEnum<<16)|(intensity<<8)|ampIndex (soundEnum 0=stop)
            KeyLedMatrixMessage,                                                                //	32-bit, LED Matrix mode, see "led_matrix_controller.h"
            KeyLedMatrixMessageFront,                                                           //	32-bit, LED Matrix mode, see "led_matrix_controller.h"
            KeyLedMatrixMessageRear,                                                            //  32-bit, LED Matrix mode, see "led_matrix_controller.h"
            KeyDualPcbState,                                                                    //  32-bit, dest address << 25 | source address << 18 | 18-bit state
            KeyPrimaryPattern,                                                                  //  32-bit, area_cut << 24 | color_variant <<  23 | intensity << 16 | variant << 13 | pattern_enumeration
            KeyRequestUpdateHardwareRevision,                                                   //  4 ASCII character hardware revision
            KeyProcessorTemperature,                                                            //  32-bit signed value in degrees Celsius
            KeyGeneralDebugData0,                                                               //  General purpose debug data, format determined per-device
            KeyGeneralDebugData1,                                                               //  General purpose debug data, format determined per-device
            KeyGeneralDebugData2,                                                               //  General purpose debug data, format determined per-device
            KeyGeneralDebugData3,                                                               //  General purpose debug data, format determined per-device
            KeyGeneralDebugData4,                                                               //  General purpose debug data, format determined per-device
            KeyGeneralDebugData5,                                                               //  General purpose debug data, format determined per-device
            KeyGeneralDebugData6,                                                               //  General purpose debug data, format determined per-device
            KeyGeneralDebugData7,                                                               //  General purpose debug data, format determined per-device
            KeyGeneralDebugData8,                                                               //  General purpose debug data, format determined per-device
            KeyGeneralDebugData9,                                                               //  General purpose debug data, format determined per-device

            //	indexed four-byte sequencer region
            //
            KeySequencer = Subreg_Base__Indexed_Sequencer_Four_Byte,

            //	C3Net																			//	Value format
            //																					//	===================================================
            KeyRequestC3NetNodeLightEngineTestFlashOn = Subreg_Base__Named_Four_Byte_C3Net,     //	29-bit, the C3Net node location ID
            KeyRequestC3NetNodeLightEngineTestFlashOff,                                         //	29-bit, the C3Net node location ID
            KeyRequestC3NetNodeInvokeBootloaderApp,                                             //	29-bit, the C3Net node location ID
            KeyRequestC3NetNodeInvokeBootloaderPattern,                                         //	29-bit, the C3Net node location ID
            KeyResponseC3NetNodeBootloaderComplete,                                             //	29-bit, the C3Net node location ID
            KeyResponseC3NetNodeBootloaderError,                                                //	29-bit, the C3Net node location ID
            KeyRequestC3NetNodeAppVersion,                                                      //	29-bit, the C3Net node location ID
            KeyResponseC3NetNodeAppVersion,                                                     //  Firmware Version [MMM][mm] M = major version, m = minor version
            KeyRequestC3NetNodeBootVersion,                                                     //  ??  PLEASE FILL IN !! 
            KeyResponseC3NetNodeBootVersion,                                                    //  ??  PLEASE FILL IN !!
            KeyRequestC3NetNodeUpdateTempCalValue,                                              //  ??  PLEASE FILL IN !!
            KeyResponseC3NetNodeUpdateTempCalValue,                                             //  ??  PLEASE FILL IN !!
            KeyRequestC3NetNodeTemperature,                                                     //  ??  PLEASE FILL IN !!
            KeyResponseC3NetNodeTemperature,                                                    //  ??  PLEASE FILL IN !!
            KeyResponseC3NetNodesAssignLocationIds,                                             //  ??  PLEASE FILL IN !!
            KeyResponseC3NetNodesClearLocationIds,                                              //  ??  PLEASE FILL IN !!
            KeyResponseC3NetPowerCycle,                                                         //  ??  PLEASE FILL IN !!
            KeyResponseC3NetNodeGenerateProductEPA,												//	Response from LBC that a new product EPA has been generated
            KeyRequestC3NetNodeFactoryDefaultReset,                                             //  Request from LBC to Reset the LE to factory default values.
            KeyResponseC3NetNodeFactoryDefaultReset,				                            //  Response to LBC that the LE has been reset to factory default values.

            //	named zero-byte, no token value													//	Description
            //																					//	===================================================
            KeyRequestAppFirmwareCrc = Region_Base__Named_Zero_Byte,                            //  Request the application firmware CRC
            KeyRequestBootloaderFirmwareCrc,                                                    //  Request the bootloader firmware CRC
            KeyRequestC3NetNodesAssignLocationIds,                                              //	Request LBC assign loc. IDs from inventory file to all LEs
            KeyRequestC3NetNodesClearLocationIds,                                               //	Request LBC clear location IDs
            KeyRequestC3NetNodeGenerateProductEPA,												//	Request LBC to generate a new product EPA
            KeyRequestC3NetPowerCycle,
            KeyRequestLockAddress,                                                              //  Tells a device (or devices if sent to global destination addr) to save its CAN address
            
            //	indexed sequencer pattern and intensity											//	Value format
            //																					//	===================================================
            KeyIndexedSequencer = Region_Base__Indexed_Sequencer_Three_Byte,					//	(intensity << 16) | pattern_enumeration

            //	ftp requests
            KeyRequestFileIndexedInfo = Region_Base__FTP_Requests,                              //  Complex format, see documentation
            KeyRequestFileInfo,
            KeyRequestFileReadStart,
            KeyRequestFileReadSegment,
            KeyRequestFileWriteStart,
            KeyRequestFileWriteSegment,
            KeyRequestFileDelete,
            KeyRequestFileTransferComplete,
            KeyRequestFileWriteFixedSegment,
            KeyRequestFileEraseFixedSegment,

            //	ftp responses
            KeyResponseFileIndexedInfo = Region_Base__FTP_Responses,                            //  Complex format, see documentation
            KeyResponseFileInfo,
            KeyResponseFileInfoComplete,
            KeyResponseFileReadStart,
            KeyResponseFileReadSegment,
            KeyResponseFileReadComplete,
            KeyResponseFileWriteStart,
            KeyResponseFileWriteSegment,
            KeyResponseFileWriteComplete,
            KeyResponseFileDelete,
            KeyResponseFileDeleteComplete,
            KeyResponseFileNotFound,
            KeyResponseFileChecksumError,
            KeyResponseFtpDiskFull,
            KeyResponseFtpClientError,
            KeyResponseFtpServerBusy,
            KeyResponseFtpServerError,
            KeyResponseFtpTransactionComplete,
            KeyResponseFtpTransactionTimedOut,
            KeyResponseFileWriteFixedSegment,
            KeyResponseFileEraseFixedSegment,
        }

        /// <summary>
        /// Token type key prefixes.
        /// </summary>
        public enum KeyPrefix
        {
            //	token key prefixes
            Command = 0x00,
            OutputStatus = 0x20,
            InputStatus = 0x40,
            BinaryRepeat = 0x60,
            AnalogRepeat = 0x80,
            PatternSync = 0xA0,
            Mask = 0xE0,

            //	pattern prefixes
            PatternPrefix_PatternWithRepeats = 0xA0,
            PatternPrefix_PatternStepWithPeriod = 0xB0,
            PatternPrefix_PatternStepWithRepeatsOfNestedPattern = 0xC0,
            PatternPrefix_PatternStepWithAllOff = 0xD0,
            PatternPrefix_PatternSectionStartWithRepeats = 0xE0,
            PatternPrefix_PatternSectionEnd = 0xF0,
            PatternPrefix_Mask = 0xF0,
        }

        /// <summary>
        /// Token values for certain requests.
        /// </summary>
        public const UInt32 TOKEN_VALUE_SYSTEM_REBOOT = 0x4C7E146F;
        public const UInt32 TOKEN_VALUE_INVOKE_BOOTLOADER = 0x5633870B;
        public const UInt32 TOKEN_VALUE_ERASE_APP_FIRMWARE = 0x6A783B52;
        public const UInt32 TOKEN_VALUE_ERASE_ALL_FIRMWARE = 0xB8E0123C;
        public const UInt32 TOKEN_VALUE_ALLOW_STATUS = 0x320CCAEC;
        public const UInt32 TOKEN_VALUE_SUPPRESS_STATUS = 0xA7CA28EE;

        public static KeyPrefix Key_GetPrefix(Keys key)
        {
            return (KeyPrefix)(int)(((int)key >> 8) & (int)KeyPrefix.Mask);
        }

        /// <summary>
        /// Returns a token key without the prefix.
        /// </summary>
        /// <param name="key">The token key.</param>
        /// <returns>The token key without the prefix.</returns>
        public static Keys Key_WithoutPrefix(Keys key)
        {
            return (Keys)((int)key & ~((int)KeyPrefix.Mask << 8));
        }

        /// <summary>
        /// Returns a value indicating whether the given key is an fpt response key.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the given key is within the specified area.</returns>
        public static bool Key_IsFtpResponse(UInt16 key)
        {
            key &= 0x1fff;
            return ((key >= Region_Base__FTP_Responses) &&
             (key < (Region_Base__FTP_Responses + Region_Size__FTP_Response)));
        }

        /// <summary>
        /// Returns a value indicating whether the given key is a command response key.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the given key is within the specified area.</returns>
        public static bool Key_IsCommand(UInt16 key)
        {
            return ((key & ~0x1fff) == 0);
        }

        /// <summary>
        /// Returns a value indicating a key value size.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>The key value size.</returns>
        public static UInt16 Key_ValueSize(UInt16 key)
        {
            key &= 0x1fff;
            if (key == 0)
                return 0;

            //  local variables
            else if (Region_Base__Indexed_One_Byte_Inputs > key)
            {
                if (Subreg_Base__Local_Two_Byte > key)
                    return 1;
                else if (Subreg_Base__Local_Four_Byte > key)
                    return 2;
                else if (Subreg_Base__Local_Zero_Byte > key)
                    return 4;
                return 0;
            }

            //  public variables
            else if (Region_Base__Named_Two_Byte > key)
                return 1;
            else if (Region_Base__Named_Four_Byte > key)
                return 2;
            else if (Region_Base__Named_Zero_Byte > key)
                return 4;
            else if (Region_Base__Indexed_Sequencer_Three_Byte > key)
                return 0;
            else if (Region_Base__FTP_Requests > key)
                return 3;
            return 0;
        }

        /// <summary>
        /// Value size property.
        /// </summary>
        /// <returns>The value size for the given token key.</returns>
        public UInt16 ValueSize
        {
            get
            {
                UInt16 key = (UInt16)((int)this.key & 0x1fff);
                return Token.Key_ValueSize(key);
            }
        }


    }
}
