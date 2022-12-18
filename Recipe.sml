Name: "SyntheticProjects"
Language: "C#|0.1"
Version: "1.0.0"
Type: "Executable"

Source: [
	"Soup/CppProjectGenerator.cs"
	"Soup/CSharpProjectGenerator.cs"
	"Xmake/CppProjectGenerator.cs"
	"Program.cs"
]
Dependencies: {
	Runtime: [ "Opal@1.1.0" ]
}
