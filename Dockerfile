FROM heroku/cedar:14

RUN apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF; \
    echo "deb http://download.mono-project.com/repo/debian wheezy main" | tee /etc/apt/sources.list.d/mono-xamarin.list; \
    apt-get update; \
    apt-get install -y \
	mono-devel \
	fsharp \
	nuget \
	nunit-console \
	nodejs \
	npm; \
    apt-get clean; \
    rm -rf /var/lib/apt/lists/* /tmp/* /var/tmp/*;

ADD . /bb

RUN cd /bb;\
    nuget restore;\
    ls;\
    xbuild /p:Configuration=Release;

CMD /bin/sh
