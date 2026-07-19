#!/usr/bin/env bash
# Compile ailobby.sp with the SourceMod 1.12 compiler inside a throwaway
# container (no local toolchain needed), then drop the .smx into the server
# overlay directory that docker-compose bind-mounts into the game install.
set -euo pipefail
cd "$(dirname "$0")"

SM_BRANCH="${SM_BRANCH:-1.12}"

docker run --rm -v "$PWD":/work -w /work --platform linux/amd64 debian:bookworm-slim bash -ec '
  dpkg --add-architecture i386
  apt-get update -qq
  apt-get install -y -qq --no-install-recommends wget ca-certificates lib32z1 lib32stdc++6 >/dev/null
  SM_TARBALL=$(wget -qO- "https://sm.alliedmods.net/smdrop/'"$SM_BRANCH"'/sourcemod-latest-linux")
  echo "Using SourceMod build: $SM_TARBALL"
  wget -q "https://sm.alliedmods.net/smdrop/'"$SM_BRANCH"'/$SM_TARBALL" -O /tmp/sm.tar.gz
  mkdir -p /tmp/sm && tar xzf /tmp/sm.tar.gz -C /tmp/sm addons/sourcemod/scripting
  cd /tmp/sm/addons/sourcemod/scripting
  ./spcomp /work/ailobby.sp -o /work/ailobby.smx
'

mkdir -p ../server/addons-overlay/sourcemod/plugins
cp ailobby.smx ../server/addons-overlay/sourcemod/plugins/ailobby.smx
echo "OK: server/addons-overlay/sourcemod/plugins/ailobby.smx"
