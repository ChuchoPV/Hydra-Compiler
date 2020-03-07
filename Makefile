hydra.exe: Driver.cs Scanner.cs Token.cs TokenCategory.cs
	mcs -out:hydra.exe Driver.cs Scanner.cs Token.cs TokenCategory.cs

clean: 
	rm hydra.exe