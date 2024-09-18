Name: 'SyntheticProjects'
Language: 'C#|0'
Version: '1.0.0'
Type: 'Executable'

Source: [
	'Soup/CppProjectGenerator.cs'
	'Soup/CSharpProjectGenerator.cs'
	'Xmake/CppProjectGenerator.cs'
	'Program.cs'
]
Dependencies: {
	Runtime: [ 'mwasplund|Opal@1' ]
}
