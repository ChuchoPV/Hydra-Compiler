hydra.exe: Driver.cs Scanner.cs Token.cs TokenCategory.cs Parser.cs \
	SyntaxError.cs

	mcs -out:hydra.exe Driver.cs Scanner.cs Token.cs TokenCategory.cs \
	Parser.cs SyntaxError.cs Node.cs SpecificNodes.cs

clean:
	rm hydra.exe