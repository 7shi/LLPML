print "[bits 32]\n";
while (<>)
{
	next if !/\.Test\("(.+?)"/;
	print "$1\n";
}
