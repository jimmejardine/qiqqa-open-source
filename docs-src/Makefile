#
# Using this Makefile as for some unfathomable reason running `npm` from the Visual Studio NMake Project's commandline always produces error 127 (error -1)
# 
#-------------------------------------------------------------------------------------------------------
# **Only when run inside Visual Studio (using Build command) do we get this error:
#
# 1>npm run docs:build
# 1>
# 1>> qiqqa@82.0.7357-40407 docs:build W:\Projects\sites\library.visyond.gov\80\lib\tooling\qiqqa
# 1>> bash -c './docs-src/site-builder.sh -b'
# 1>
# 1>Wnpm ERR! code ELIFECYCLE
# 1>npm ERR! errno 4294967295
# 1>npm ERR! qiqqa@82.0.7357-40407 docs:build: `bash -c './docs-src/site-builder.sh -b'`
# 1>npm ERR! Exit status 4294967295
# 1>npm ERR!
# 1>npm ERR! Failed at the qiqqa@82.0.7357-40407 docs:build script.
# 1>npm ERR! This is probably not a problem with npm. There is likely additional logging output above.
#-------------------------------------------------------------------------------------------------------
#


all: build

build:
	#npm run docs:build
	bash -c './docs-src/site-builder.sh -b'
	# !@#$%^&*() samee problem (error -1) when invoking the bash script directly from VS2019 Build command. WTF?!?!?!?


.PHONY: all build

