FROM alpine:latest

RUN apk add --no-cache --virtual .dev-dependencies \
    libstdc++ \
    icu-libs \
    zsh \
    zsh-vcs \
    git \
    curl \
    dotnet8-sdk \
    dotnet-zsh-completion
    

SHELL ["/bin/zsh", "-c"]

RUN touch /root/.zshrc

RUN zsh <(curl -s https://raw.githubusercontent.com/zap-zsh/zap/master/install.zsh) --branch release-v1

WORKDIR /workspace

COPY . .
