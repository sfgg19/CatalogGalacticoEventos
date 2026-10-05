
# 🌌 Catálogo Galáctico API

API REST construida con **ASP.NET Core Minimal API** y **.NET 10** para la gestión y simulación de un catálogo inspirado en el universo galáctico: personajes, cartas coleccionables y eventos históricos con almacenamiento en memoria.

---

## 🚀 Requisitos e Instalación

1. **Requisitos**: [.NET 10 SDK](https://dotnet.microsoft.com/) y Visual Studio 2022 / VS Code.
2. **Clonar y compilar**:
   ```bash
   git clone <URL_DEL_REPOSITORIO>
   cd CatalogoGalactico
   dotnet build

3. **Ejecutar**:
```bash
dotnet run --project CatalogoGalactico

```


4. **Documentación Swagger UI**:
Navega a `https://localhost:<puerto>/swagger` para probar interactivamente cada endpoint.

---

## 📂 Estructura del Proyecto

```text
CatalogoGalactico/
├── Data/
│   └── GalacticContext.cs       # Almacenamiento en memoria y datos iniciales
├── Endpoints/
│   ├── PersonajeEndpoints.cs    # Mapeo y controladores de /api/personajes
│   ├── CartaEndpoints.cs        # Mapeo y controladores de /api/cartas
│   └── EventoEndpoints.cs       # Mapeo y controladores de /api/eventos
├── Models/
│   ├── Entities.cs              # Records de dominio (Personaje, CardPersonaje, Evento)
│   └── Responses.cs             # Wrappers para ApiResponse y ApiErrorResponse
├── Services/
│   └── BatallaServices.cs       # Lógica de cálculo, MVP y simulación de batallas
├── .gitignore                   # Exclusión de carpetas bin, obj y temporales
├── Program.cs                   # Pipeline HTTP, IoC y configuración JSON
└── README.md

```

---

## 📋 Reglas de Negocio y Convenciones

* **Convención Temporal**: Años cronológicos numéricos enteros:
* `10 BBY` = `-10`
* `Batalla de Yavin` = `0`
* `4 ABY` = `4`


* **Facciones permitidas**: `Rebelde`, `Imperio`, `Neutral`.
* **Estados de personaje**: `Vivo`, `Muerto`, `Desconocido`.
* **Simulación de Batalla**: Suma el poder de las cartas de los participantes por bando y aplica un factor aleatorio controlado (±10%).
* **Consistencia de Estado**: Personajes con estado `Muerto` no aportan puntos en simulaciones de eventos posteriores.

---

## 📡 Formato Estándar de Respuestas

Todas las respuestas están estructuradas de forma uniforme en `camelCase`:

### Respuesta Exitosa

```json
{
  "data": [ ... ],
  "meta": {
    "page": 1,
    "limit": 10,
    "total": 10
  }
}

```

### Respuesta de Error

```json
{
  "error": {
    "codigo": "NOT_FOUND",
    "mensaje": "Recurso no encontrado",
    "detalles": null
  }
}

```

---

## 🛠️ Catálogo de Endpoints

| Recurso | Método | Ruta | Descripción |
| --- | --- | --- | --- |
| **Personajes** | `GET` | `/api/personajes` | Lista todos los personajes |
|  | `GET` | `/api/personajes/{id}` | Obtiene personaje por ID |
|  | `POST` | `/api/personajes` | Registra un nuevo personaje |
|  | `PUT` | `/api/personajes/{id}` | Actualiza un personaje |
|  | `DELETE` | `/api/personajes/{id}` | Elimina un personaje |
| **Cartas** | `GET` | `/api/cartas` | Lista todas las cartas coleccionables |
|  | `GET` | `/api/cartas/{id}` | Obtiene carta por ID |
|  | `POST` | `/api/cartas` | Crea una nueva carta |
|  | `PUT` | `/api/cartas/{id}` | Actualiza datos de la carta |
|  | `DELETE` | `/api/cartas/{id}` | Elimina una carta |
| **Eventos** | `GET` | `/api/eventos` | Lista todos los eventos registrados |
|  | `GET` | `/api/eventos/{id}` | Obtiene evento por ID |
|  | `POST` | `/api/eventos` | Crea un nuevo evento |
|  | `PUT` | `/api/eventos/{id}` | Actualiza un evento existente |
| **Acciones** | `GET` | `/api/eventos/{id}/mvp` | Retorna el participante con mayor poder (MVP) |
|  | `POST` | `/api/eventos/{id}/simular` | Simula la batalla entre Rebeldes e Imperio |

```

<FollowUp>
¿Deseas que agregue la sección de ejemplos con los JSON listos para copiar y pegar en las pruebas de Swagger?
</FollowUp>

```
