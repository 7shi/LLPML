# Makefile for Mono (requires 2.0)

CSC = gmcs

all: bin/Compiler.exe

bin/Compiler.exe: bin/LLPML.dll
	$(CSC) /out:$@ `find Compiler -name "*.cs"` /r:bin/CompilerLib.dll /r:bin/LLPML.dll

bin/LLPML.dll: bin/CompilerLib.dll
	$(CSC) /out:$@ /t:library `find LLPML -name "*.cs"` /r:bin/CompilerLib.dll

bin/CompilerLib.dll:
	mkdir -p bin
	$(CSC) /out:$@ /t:library `find CompilerLib -name "*.cs"`

clean:
	rm -rf bin obj

####

compiler: bin/COMPILER.exe

LIBSRC = LLPML/Properties/AssemblyInfo.cs `find CompilerLib LLPML -name "*.cs" | grep -v /Properties/`

bin/COMPILER.exe:
	mkdir -p bin
	$(CSC) /out:$@ $(LIBSRC) `find Compiler -name "*.cs"`

####

BATCH = utils/build.bat

batch:
	echo cd .."\r" > $(BATCH)
	echo %WINDIR%/Microsoft.NET/Framework/v3.5/csc -o+ -out:utils/COMPILER.exe $(LIBSRC) `find Compiler -name "*.cs"`"\r" | sed 's/\//\\/g' >> $(BATCH)
	chmod 755 $(BATCH)
