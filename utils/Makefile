all: 16.cs 8.cs

16.cs: 16-tmp.cs 16.txt
	perl merge.pl $^ > $@
8.cs: 8-tmp.cs 8.txt
	perl merge.pl $^ > $@

16.txt: 16
	ndisasm -u $< > $@
8.txt: 8
	ndisasm -u $< > $@

16: 16.asm
	nasm $<
8: 8.asm
	nasm $<

16.asm: 16-tmp.cs
	perl extract.pl < $< > $@
8.asm: 8-tmp.cs
	perl extract.pl < $< > $@

16-tmp.cs: 32.cs
	perl srcconv16.pl < $< > $@
8-tmp.cs: 32.cs
	perl srcconv8.pl < $< > $@

clean:
	rm -f 16.cs 16.txt 16 16.asm 16-tmp.cs
	rm -f 8.cs 8.txt 8 8.asm 8-tmp.cs
