# MMOTFG - API para programar juegos de rol en Telegram

MMOTFG proporciona código que sirve como base para la creación de juegos RPG multijugador a través de plataformas de mensajería como Telegram, con personajes con distintas estadísticas, equipamiento, objetos consumibles, batallas contra enemigos, así como soporte para creación de mapas de juego en los que cada sala puede contener distintos eventos.

## Requisitos iniciales

- .NET Core 3.1 o superior
- Docker
- Una base de datos en la que poder guardar los datos de los juegos (por defecto Firebase)

## Paso a paso: flujo de trabajo de MMOTFG

MMOTFG permite hacer videojuegos RPG en Telegram en los cuales los usuarios van moviéndose por un mapa y luchando contra enemigos mediante comandos de texto. MMOTFG proporciona un motor extensible en el que se puede hacer esto mediante los siguientes pasos:

- Para hacer un mapa de juego personalizado, hay que crear un archivo map.json y meterlo en la carpeta assets. Este archivo json contiene información de todos los nodos del mapa, con la siguiente sintaxis:
```
[
  {
    "Name": "NombreDelNodo",
    "NodeConnections":{
      "Direccion1":{"ConnectingNode": "OtroNodo1"},
      "Direccion2":{"ConnectingNode": "OtroNodo2"}
    },
    "OnArriveEvent": [
      {
      "EventType":"eSendText",
      "Description": "Texto a enviar"
      }
    ],
    "OnInspectEvent": [
      {
        "EventType": "eSendImage",
        "ImageName": "imagen.jpg",
        "Description": "Texto que acompaña a la imagen"
      },
      {
        "EventType": "eSendtext",
        "Description": "OtroTexto"
      },
      {
        "EventType":"eGiveItem",
        "ItemLots": [
          {
            "Item": "nombre_item",
            "Quantity": 1,
            "ChanceToObtain": 1
          },
          {
            "Item": "nombre_item2",
            "Quantity": 1,
            "ChanceToObtain": 1
          },
        ...
        ]
      }
    ],
    "OnExitEvent": [
        ...
    ]
  },
  ...
]
```
Los distintos eventos que puede tener un nodo son OnArriveEvent, OnInspectEvent y OnExitEvent, que se ejecutarán cuando un usuario llegue a un nodo, lo inspeccione y se vaya, respectivamente.
Cada uno de estos eventos podrá contener un número indefinido de acciones, que pueden ser de distintos tipos:  
**eSendText**: Envía un texto al jugador, definido en el campo Description.  
**eSendImage**: Envía una imagen al jugador, cuyo nombre de archivo estará definido en el campo ImageName. Se puede acompañar de un texto al añadirlo en el campo Description.   
**eSendAudio**: Envía un archivo de audio al jugador, cuyo nombre de archivo estará definido en el campo AudioName. Se puede acompañar de un texto al añadirlo en el campo Description.  
**eGiveItem**: Le da objetos al jugador. Los objetos deben estar dentro de una lista llamada ItemLots, en la que cada elemento contendrá un campo Item con el nombre del objeto en el campo Item, la cantidad en el campo Quantity y las posibilidades de que el jugador reciba el objeto en el campo ChanceToObtain.  
**eStartBattle**: Comienza una batalla contra un enemigo, cuyo nombre irá en el campo Enemy. 

- El proyecto se puede ejecutar y compilar directamente o ejecutarse mediante un contenedor de Docker con el script dockerBuildandRunBot.bat. Este script utiliza MSBuild para compilar el proyecto mediante la configuración proporcionada por el Dockerfile que está dentro del proyecto y posteriormente lanza un contenedor configurado para que contenga también todos los archivos de la caperta assets. Esto es especialmente útil a la hora de alojar un bot creado con MMOTFG.

- Comandos y cómo funcionan
- Sección de cómo manejar la base de datos
- Cómo crear items
- Cómo crear enemigos

## Alojamiento del proyecto en un servidor externo

- Aquí hablar de cómo hay que logearse en el repositorio/servidor con docker, hacer push y ejemplo de cómo se configura en Azure
