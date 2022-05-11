#!/bin/bash
# -- ENCODING: UTF-8 --
echo "---Numero de lineas"
unset lineasA
unset lineasB
lineasA="$(wc -l < a.txt)"
lineasB="$(wc -l < b.txt)"
((lineasA++))
((lineasB++))
echo $lineasA
echo $lineasB

if [ "$lineasA" = "$lineasB" ]; then
echo "---Diff"
diff a.txt b.txt > output.txt

echo "---Numero de lineas distintas"
unset lineasIncorrectas
lineasIncorrectas="$(grep -c ">" output.txt)"
echo $lineasIncorrectas

    echo "---Porcentaje de parentesco :D"
    if [ "$lineasIncorrectas" != "0" ]; then
    unset porcentaje
    porcentaje="$(printf %.2f "$((10**3 * (lineasB-lineasIncorrectas)/lineasB))e-3")"
    ((porcentaje=porcentaje+0))
    echo $porcentaje "%"

    else
    echo "100 %"
    fi

else
echo "El tamano de los archivos no coincide"
exit $ERRCODE
fi

exit