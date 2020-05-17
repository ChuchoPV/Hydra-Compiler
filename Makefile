hydra.exe: Driver.cs Scanner.cs Token.cs TokenCategory.cs Parser.cs \
	SyntaxError.cs Node.cs SpecificNodes.cs SemanticAnalyzer.cs \
	SemanticError.cs Tables.cs

	mcs -out:hydra.exe Driver.cs Scanner.cs Token.cs TokenCategory.cs \
	Parser.cs SyntaxError.cs Node.cs SpecificNodes.cs SemanticAnalyzer.cs \
	SemanticError.cs Tables.cs

clean:
	rm hydra.exe