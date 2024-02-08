{
	"Name": "Named Quad Flash Patterns",
	"Expressions": [{
			"Name": "Steady/Cut",
			"RegStandard": 0,
			"ExpressionEnum": 1026,
			"Repeats": 0,
			"InputPriority": 1,
			"OutputPriority": 0,
			"Sequencer": 0,
			"Value": 40,
			"Areas": [{
					"Name": "Steady/Cut Area 1",
					"Key": 500,
					"DefaultValue": 0,
					"Outputs": [],
					"NameWithOutputPaths": "Steady/Cut Area 1"
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
		}, {
			"Name": "Quad Flash SS 75 FPM",
			"RegStandard": 0,
			"ExpressionEnum": 1038,
			"Repeats": 0,
			"InputPriority": 1,
			"OutputPriority": 0,
			"Sequencer": 0,
			"Value": 100,
			"Areas": [{
					"Name": "Quad Flash SS 75 FPM Area 1",
					"Key": 500,
					"DefaultValue": 0,
					"Outputs": [],
					"NameWithOutputPaths": "Quad Flash SS 75 FPM Area 1"
				}, {
					"Name": "Quad Flash SS 75 FPM Area 2",
					"Key": 501,
					"DefaultValue": 0,
					"Outputs": [],
					"NameWithOutputPaths": "Quad Flash SS 75 FPM Area 2"
				}
			],
			"Entries": [{
					"Repeats": 3
				}, {
					"Period": 400,
					"Tokens": [{
							"Key": 500,
							"Value": 100
						}
					]
				}, {
					"Period": 400,
					"Tokens": [{
							"Key": 500,
							"Value": 0
						}
					]
				}, {}, {
					"Period": 400,
					"Tokens": [{
							"Key": 500,
							"Value": 100
						}
					]
				}, {
					"Period": 1200,
					"Tokens": [{
							"Key": 500,
							"Value": 0
						}
					]
				}, {
					"Repeats": 3
				}, {
					"Period": 400,
					"Tokens": [{
							"Key": 501,
							"Value": 100
						}
					]
				}, {
					"Period": 400,
					"Tokens": [{
							"Key": 501,
							"Value": 0
						}
					]
				}, {}, {
					"Period": 400,
					"Tokens": [{
							"Key": 501,
							"Value": 100
						}
					]
				}, {
					"Period": 1200,
					"Tokens": [{
							"Key": 501,
							"Value": 0
						}
					]
				}
			]
		}
	]
}
