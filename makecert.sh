#!/bin/bash
fname=test
fnamep="$fname.pem"
fnameo="$fname.pfx"
openssl req -new -newkey rsa:1024 -days 3650 -subj '/CN=cardgameserver/O=ian/C=US' -nodes -x509 -keyout "$fnamep" -out "$fnamep"
openssl pkcs12 -export -in "$fnamep" -inkey "$fnamep" -out "$fnameo"
rm "$fnamep"
