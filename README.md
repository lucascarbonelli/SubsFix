# SubsFix

SubsFix es una aplicación de consola de comandos programada en C#.

SubsFix manipula archivos de subtítulos de extensión srt,
permitiendo corregir los tiempos de aparición y despación de los subtitulos.

El usuario debe especificar la cantidad de segundos que están corridos el primer y
el último subtítulo, y SubsFix se encargará de modificar los tiempos del resto de
los subtítulos apropiadamente.

Para ejecutar SubsFix escriba los parámetros correspondientes como se indica a continuación:

SubsFix.exe [archivo.srt] [corrimiento en segundos del primer subtitulo] [corrimiento en segundos del ultimo subtitulo] [opcional: codifición]

Opciones de codificación del archivo: 
ansi 
utf-8 
unicode 
unicodeBE (unicode big endian)
