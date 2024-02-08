{
	"Expressions": [{
			"Name": "Quad Flash SS 75 FPM",
			"RegStandard": 0,
			"ExpressionEnum": 3,
			"Repeats": 0,
			"InputPriority": 1,
			"OutputPriority": 1,
			"Sequencer": 0,
			"Value": 100,
			"Areas": [{
					"Name": "Quad Flash SS 75 FPM Area 1",
					"Key": 500,
					"DefaultValue": 0,
					"Outputs": [{
							"Path": "15356/Red/0",
							"Value": 100
						}, {
							"Path": "1021/Red/0",
							"Value": 100
						}, {
							"Path": "1022/Red/0",
							"Value": 100
						}
					]
				}, {
					"Name": "Quad Flash SS 75 FPM Area 2",
					"Key": 501,
					"DefaultValue": 0,
					"Outputs": [{
							"Path": "2/Blue/0",
							"Value": 100
						}, {
							"Path": "3/Blue/0",
							"Value": 100
						}, {
							"Path": "2052/Blue/0",
							"Value": 100
						}
					]
				}
			],
			"Entries": [{
					"Repeats": 3
				}, {
					"Period": 40,
					"Tokens": [{
							"Key": 500,
							"Value": 100
						}
					]
				}, {
					"Period": 40,
					"Tokens": [{
							"Key": 500,
							"Value": 0
						}
					]
				}, {}, {
					"Period": 40,
					"Tokens": [{
							"Key": 500,
							"Value": 100
						}
					]
				}, {
					"Period": 120,
					"Tokens": [{
							"Key": 500,
							"Value": 0
						}
					]
				}, {
					"Repeats": 3
				}, {
					"Period": 40,
					"Tokens": [{
							"Key": 501,
							"Value": 100
						}
					]
				}, {
					"Period": 40,
					"Tokens": [{
							"Key": 501,
							"Value": 0
						}
					]
				}, {}, {
					"Period": 40,
					"Tokens": [{
							"Key": 501,
							"Value": 100
						}
					]
				}, {
					"Period": 120,
					"Tokens": [{
							"Key": 501,
							"Value": 0
						}
					]
				}
			]
		}, {
			"Name": "Triple Pop 75 FPM",
			"RegStandard": 0,
			"ExpressionEnum": 4,
			"Repeats": 0,
			"InputPriority": 1,
			"OutputPriority": 1,
			"Sequencer": 0,
			"Value": 100,
			"Areas": [{
					"Name": "Triple Pop 75 FPM Area 1",
					"Key": 500,
					"DefaultValue": 0,
					"Outputs": [{
							"Path": "1021/Red/0",
							"Value": 100
						}, {
							"Path": "1022/Red/0",
							"Value": 100
						}, {
							"Path": "0/Red/0",
							"Value": 100
						}, {
							"Path": "2/Blue/0",
							"Value": 100
						}, {
							"Path": "3/Blue/0",
							"Value": 100
						}
					]
				}
			],
			"Entries": [{
					"Repeats": 2
				}, {
					"Period": 40,
					"Tokens": [{
							"Key": 500,
							"Value": 100
						}
					]
				}, {
					"Period": 120,
					"Tokens": [{
							"Key": 500,
							"Value": 0
						}
					]
				}, {}, {
					"Period": 120,
					"Tokens": [{
							"Key": 500,
							"Value": 100
						}
					]
				}, {
					"Period": 360,
					"Tokens": [{
							"Key": 500,
							"Value": 0
						}
					]
				}
			]
		}, {
			"Name": "Arrow 8 50",
			"RegStandard": 0,
			"ExpressionEnum": 6,
			"Repeats": 0,
			"InputPriority": 1,
			"OutputPriority": 1,
			"Sequencer": 3,
			"Value": 100,
			"Areas": [{
					"Name": "Arrow 8 50 Area 1",
					"Key": 500,
					"DefaultValue": 0,
					"Outputs": [{
							"Path": "13307/Red/0",
							"Value": 100
						}
					]
				}, {
					"Name": "Arrow 8 50 Area 2",
					"Key": 501,
					"DefaultValue": 0,
					"Outputs": [{
							"Path": "15356/Red/0",
							"Value": 100
						}
					]
				}, {
					"Name": "Arrow 8 50 Area 3",
					"Key": 502,
					"DefaultValue": 0,
					"Outputs": [{
							"Path": "1021/Red/0",
							"Value": 100
						}
					]
				}, {
					"Name": "Arrow 8 50 Area 4",
					"Key": 503,
					"DefaultValue": 0,
					"Outputs": [{
							"Path": "1022/Red/0",
							"Value": 100
						}
					]
				}, {
					"Name": "Arrow 8 50 Area 5",
					"Key": 504,
					"DefaultValue": 0,
					"Outputs": [{
							"Path": "2/Blue/0",
							"Value": 100
						}
					]
				}, {
					"Name": "Arrow 8 50 Area 6",
					"Key": 505,
					"DefaultValue": 0,
					"Outputs": [{
							"Path": "3/Blue/0",
							"Value": 100
						}
					]
				}, {
					"Name": "Arrow 8 50 Area 7",
					"Key": 506,
					"DefaultValue": 0,
					"Outputs": [{
							"Path": "2052/Blue/0",
							"Value": 100
						}
					]
				}, {
					"Name": "Arrow 8 50 Area 8",
					"Key": 507,
					"DefaultValue": 0,
					"Outputs": [{
							"Path": "4101/Blue/0",
							"Value": 100
						}
					]
				}
			],
			"Entries": [{
					"Period": 50,
					"Tokens": [{
							"Key": 507,
							"Value": 0
						}, {
							"Key": 500,
							"Value": 100
						}
					]
				}, {
					"Period": 50,
					"Tokens": [{
							"Key": 500,
							"Value": 0
						}, {
							"Key": 501,
							"Value": 100
						}
					]
				}, {
					"Period": 50,
					"Tokens": [{
							"Key": 501,
							"Value": 0
						}, {
							"Key": 502,
							"Value": 100
						}
					]
				}, {
					"Period": 50,
					"Tokens": [{
							"Key": 502,
							"Value": 0
						}, {
							"Key": 503,
							"Value": 100
						}
					]
				}, {
					"Period": 50,
					"Tokens": [{
							"Key": 503,
							"Value": 0
						}, {
							"Key": 504,
							"Value": 100
						}
					]
				}, {
					"Period": 50,
					"Tokens": [{
							"Key": 504,
							"Value": 0
						}, {
							"Key": 505,
							"Value": 100
						}
					]
				}, {
					"Period": 50,
					"Tokens": [{
							"Key": 505,
							"Value": 0
						}, {
							"Key": 506,
							"Value": 100
						}
					]
				}, {
					"Period": 50,
					"Tokens": [{
							"Key": 506,
							"Value": 0
						}, {
							"Key": 507,
							"Value": 100
						}
					]
				}
			]
		}, {
			"Name": "Arrow Right 8 B1-B8",
			"RegStandard": 0,
			"ExpressionEnum": 8,
			"Repeats": 0,
			"InputPriority": 1,
			"OutputPriority": 1,
			"Sequencer": 3,
			"Value": 100,
			"Areas": [{
					"Name": "Arrow 8 200 Area 1",
					"Key": 500,
					"DefaultValue": 0,
					"Outputs": [{
							"Path": "11260/Blue/0",
							"Value": 100
						}
					]
				}, {
					"Name": "Arrow 8 200 Area 2",
					"Key": 501,
					"DefaultValue": 0,
					"Outputs": [{
							"Path": "11260/Blue/0",
							"Value": 100
						}, {
							"Path": "9213/Amber/0",
							"Value": 100
						}
					]
				}, {
					"Name": "Arrow 8 200 Area 3",
					"Key": 502,
					"DefaultValue": 0,
					"Outputs": [{
							"Path": "11260/Blue/0",
							"Value": 100
						}, {
							"Path": "9213/Amber/0",
							"Value": 100
						}, {
							"Path": "9214/Amber/0",
							"Value": 100
						}
					]
				}, {
					"Name": "Arrow 8 200 Area 4",
					"Key": 503,
					"DefaultValue": 0,
					"Outputs": [{
							"Path": "11260/Blue/0",
							"Value": 100
						}, {
							"Path": "9213/Amber/0",
							"Value": 100
						}, {
							"Path": "9214/Amber/0",
							"Value": 100
						}, {
							"Path": "8192/Amber/0",
							"Value": 100
						}
					]
				}, {
					"Name": "Arrow 8 200 Area 5",
					"Key": 504,
					"DefaultValue": 0,
					"Outputs": [{
							"Path": "11260/Blue/0",
							"Value": 100
						}, {
							"Path": "9213/Amber/0",
							"Value": 100
						}, {
							"Path": "9214/Amber/0",
							"Value": 100
						}, {
							"Path": "8192/Amber/0",
							"Value": 100
						}, {
							"Path": "8194/Amber/0",
							"Value": 100
						}
					]
				}, {
					"Name": "Arrow 8 200 Area 6",
					"Key": 505,
					"DefaultValue": 0,
					"Outputs": [{
							"Path": "11260/Blue/0",
							"Value": 100
						}, {
							"Path": "9213/Amber/0",
							"Value": 100
						}, {
							"Path": "9214/Amber/0",
							"Value": 100
						}, {
							"Path": "8192/Amber/0",
							"Value": 100
						}, {
							"Path": "8194/Amber/0",
							"Value": 100
						}, {
							"Path": "8195/Amber/0",
							"Value": 100
						}
					]
				}, {
					"Name": "Arrow 8 200 Area 7",
					"Key": 506,
					"DefaultValue": 0,
					"Outputs": [{
							"Path": "11260/Blue/0",
							"Value": 100
						}, {
							"Path": "9213/Amber/0",
							"Value": 100
						}, {
							"Path": "9214/Amber/0",
							"Value": 100
						}, {
							"Path": "8192/Amber/0",
							"Value": 100
						}, {
							"Path": "8194/Amber/0",
							"Value": 100
						}, {
							"Path": "8195/Amber/0",
							"Value": 100
						}, {
							"Path": "6148/Red/0",
							"Value": 100
						}
					]
				}, {
					"Name": "Arrow 8 200 Area 8",
					"Key": 507,
					"DefaultValue": 0,
					"Outputs": [{
							"Path": "11260/Blue/0",
							"Value": 100
						}, {
							"Path": "9213/Amber/0",
							"Value": 100
						}, {
							"Path": "9214/Amber/0",
							"Value": 100
						}, {
							"Path": "8192/Amber/0",
							"Value": 100
						}, {
							"Path": "8194/Amber/0",
							"Value": 100
						}, {
							"Path": "8195/Amber/0",
							"Value": 100
						}, {
							"Path": "6148/Red/0",
							"Value": 100
						}, {
							"Path": "4101/Blue/0",
							"Value": 100
						}
					]
				}
			],
			"Entries": [{
					"Period": 200,
					"Tokens": [{
							"Key": 507,
							"Value": 0
						}, {
							"Key": 500,
							"Value": 100
						}
					]
				}, {
					"Period": 200,
					"Tokens": [{
							"Key": 500,
							"Value": 0
						}, {
							"Key": 501,
							"Value": 100
						}
					]
				}, {
					"Period": 200,
					"Tokens": [{
							"Key": 501,
							"Value": 0
						}, {
							"Key": 502,
							"Value": 100
						}
					]
				}, {
					"Period": 200,
					"Tokens": [{
							"Key": 502,
							"Value": 0
						}, {
							"Key": 503,
							"Value": 100
						}
					]
				}, {
					"Period": 200,
					"Tokens": [{
							"Key": 503,
							"Value": 0
						}, {
							"Key": 504,
							"Value": 100
						}
					]
				}, {
					"Period": 200,
					"Tokens": [{
							"Key": 504,
							"Value": 0
						}, {
							"Key": 505,
							"Value": 100
						}
					]
				}, {
					"Period": 200,
					"Tokens": [{
							"Key": 505,
							"Value": 0
						}, {
							"Key": 506,
							"Value": 100
						}
					]
				}, {
					"Period": 200,
					"Tokens": [{
							"Key": 506,
							"Value": 0
						}, {
							"Key": 507,
							"Value": 100
						}
					]
				}
			]
		}, {
			"Name": "Steady Override A1 & B1",
			"RegStandard": 0,
			"ExpressionEnum": 10,
			"Repeats": 0,
			"InputPriority": 1,
			"OutputPriority": 5,
			"Sequencer": 0,
			"Value": 80,
			"Areas": [{
					"Name": "Steady 85% Area 1",
					"Key": 500,
					"DefaultValue": 0,
					"Outputs": [{
							"Path": "1021/Red/0",
							"Value": 100
						}, {
							"Path": "1022/Red/0",
							"Value": 100
						}, {
							"Path": "9213/Amber/0",
							"Value": 100
						}, {
							"Path": "9214/Amber/0",
							"Value": 100
						}, {
							"Path": "0/Red/0",
							"Value": 100
						}, {
							"Path": "8192/Amber/0",
							"Value": 100
						}, {
							"Path": "2/Blue/0",
							"Value": 100
						}, {
							"Path": "3/Blue/0",
							"Value": 100
						}, {
							"Path": "2052/Blue/0",
							"Value": 100
						}, {
							"Path": "6148/Red/0",
							"Value": 100
						}, {
							"Path": "8195/Amber/0",
							"Value": 100
						}, {
							"Path": "8194/Amber/0",
							"Value": 100
						}
					]
				}
			],
			"Entries": [{
					"Period": 100,
					"Tokens": [{
							"Key": 500,
							"Value": 50
						}
					]
				}
			]
		}, {
			"Name": "Cut A2 & B2",
			"RegStandard": 0,
			"ExpressionEnum": 11,
			"Repeats": 0,
			"InputPriority": 1,
			"OutputPriority": 6,
			"Sequencer": 0,
			"Value": 0,
			"Areas": [{
					"Name": "Steady Area 1",
					"Key": 500,
					"DefaultValue": 0,
					"Outputs": [{
							"Path": "13307/Red/0",
							"Value": 100
						}, {
							"Path": "15356/Red/0",
							"Value": 100
						}, {
							"Path": "11260/Blue/0",
							"Value": 100
						}, {
							"Path": "1021/Red/0",
							"Value": 100
						}, {
							"Path": "9213/Amber/0",
							"Value": 100
						}
					]
				}
			],
			"Entries": [{
					"Period": 100,
					"Tokens": [{
							"Key": 500,
							"Value": 100
						}
					]
				}
			]
		}
	]
}
