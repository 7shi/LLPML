# Makefile for Mono (requires 2.0)

all: bin/Compiler.exe bin/Sample.exe

RSRC = obj/Compiler.Properties.Resources.resources

bin/Compiler.exe: bin/LLPML.dll $(RSRC)
	gmcs /out:$@ /resource:$(RSRC) `find Compiler -name "*.cs"` /r:bin/CompilerLib.dll /r:bin/LLPML.dll /r:System.Windows.Forms

$(RSRC): Compiler/Properties/Resources.resx
	mkdir -p obj
	resgen2 /useSourcePath /compile Compiler/Properties/Resources.resx,$@

bin/Sample.exe: bin/LLPML.dll
	gmcs /out:$@ /t:winexe `find Sample -name "*.cs"` /r:bin/CompilerLib.dll /r:bin/LLPML.dll /r:System.Data /r:System.Drawing /r:System.Windows.Forms
	cp -r Sample/Samples bin

bin/LLPML.dll: bin/CompilerLib.dll
	gmcs /out:$@ /t:library `find LLPML -name "*.cs"` /r:bin/CompilerLib.dll

bin/CompilerLib.dll:
	mkdir -p bin
	gmcs /out:$@ /t:library `find CompilerLib -name "*.cs"`

clean:
	rm -rf bin obj

####

compiler: bin/COMPILER.exe

LIBSRC = `find CompilerLib LLPML -name "*.cs" | grep -v /Properties/`

bin/COMPILER.exe: $(RSRC)
	mkdir -p bin
	gmcs /out:$@ /resource:$(RSRC) $(LIBSRC) `find Compiler -name "*.cs"` /r:System.Windows.Forms

