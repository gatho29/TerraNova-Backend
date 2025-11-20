# 🏠 TerraNova Backend

API RESTful desarrollada en .NET 9.0 para gestionar y consultar propiedades inmobiliarias con filtros avanzados.

## 📋 Descripción

TerraNova Backend es una API robusta y escalable que permite gestionar propiedades inmobiliarias con capacidades de búsqueda y filtrado avanzadas. Utiliza una arquitectura limpia (Clean Architecture) con separación de responsabilidades y MongoDB como base de datos.

## ✨ Características

- ✅ Consulta de propiedades con filtros múltiples
- ✅ Búsqueda por nombre (parcial, case-insensitive, sin acentos)
- ✅ Búsqueda por dirección (parcial, case-insensitive, sin acentos)
- ✅ Filtrado por rango de precios
- ✅ Estructura completa de base de datos con entidades relacionadas:
  - **Owner** (Propietarios)
  - **PropertyImage** (Imágenes de propiedades)
  - **PropertyTrace** (Historial de transacciones)
- ✅ Documentación interactiva con Swagger
- ✅ Arquitectura limpia y escalable
- ✅ Manejo robusto de errores
- ✅ Tests unitarios incluidos
- ✅ Optimización con índices MongoDB
- ✅ CORS configurado para desarrollo frontend
- ✅ Endpoints para poblar y agregar datos de prueba

## 🛠️ Tecnologías

- **.NET 9.0** - Framework de desarrollo
- **MongoDB** - Base de datos NoSQL
- **AutoMapper** - Mapeo de objetos
- **Swagger/OpenAPI** - Documentación interactiva
- **xUnit** - Framework de testing
- **Moq** - Mocking para tests

## 🗄️ Estructura de Base de Datos

La base de datos utiliza MongoDB con las siguientes colecciones:

### Colecciones

1. **Properties** - Propiedades inmobiliarias
   - Campos principales: `Id`, `IdOwner`, `Name`, `Address`, `Price`, `CodeInternal`, `Year`
   - Índices: `Name`, `Address`, `Price`, `IdOwner`, `CodeInternal` (único)

2. **Owners** - Propietarios de las propiedades
   - Campos: `IdOwner`, `Name`, `Address`, `Photo`, `Birthday`

3. **PropertyImages** - Imágenes asociadas a propiedades
   - Campos: `IdPropertyImage`, `IdProperty`, `File`, `Enabled`

4. **PropertyTraces** - Historial de transacciones de propiedades
   - Campos: `IdPropertyTrace`, `IdProperty`, `DateSale`, `Name`, `Value`, `Tax`

### Relaciones

- Una **Property** tiene un **Owner** (relación por `IdOwner`)
- Una **Property** puede tener múltiples **PropertyImages**
- Una **Property** puede tener múltiples **PropertyTraces**

Las relaciones se cargan automáticamente cuando se consulta una propiedad.

## 📦 Arquitectura

El proyecto sigue los principios de Clean Architecture con las siguientes capas:

```
┌─────────────────────────────────────┐
│   PropertyAPI (Presentación)         │
│   - Controllers                      │
│   - Program.cs                       │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│   PropertyAPI.Application            │
│   - Services (Lógica de Negocio)    │
│   - DTOs                             │
│   - Interfaces                       │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│   PropertyAPI.Domain                │
│   - Entities (Modelos de Dominio)  │
│   - Interfaces (Contratos)          │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│   PropertyAPI.Infrastructure        │
│   - Repositories (Acceso a Datos)  │
│   - Data (MongoDB Context)         │
└─────────────────────────────────────┘
```

## 🚀 Inicio Rápido

### Requisitos Previos

- [.NET SDK 9.0](https://dotnet.microsoft.com/download) o superior
- [MongoDB](https://www.mongodb.com/try/download/community) (local, o Atlas)

### Instalación

1. **Clonar el repositorio:**
```bash
git clone https://github.com/tu-usuario/TerraNova-Backend.git
cd TerraNova-Backend
```

2. **Configurar MongoDB:**
   - Opción 1: MongoDB local
   - Opción 2: MongoDB Atlas (cloud) - **[Ver guía de conexión](./CONFIGURAR_MONGODB_ATLAS.md)**

3. **Configurar la aplicación:**
   
   Edita `src/PropertyAPI/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "MongoDB": "mongodb://localhost:27017"
     },
     "MongoDB": {
       "DatabaseName": "PropertyDB"
     }
   }
   ```

4. **Restaurar dependencias:**
```bash
cd src/PropertyAPI
dotnet restore
```

5. **Ejecutar la aplicación:**
```bash
dotnet run
```

La API estará disponible en:
- HTTP: `http://localhost:5000` (desarrollo)
- HTTPS: `https://localhost:5001` (solo en producción)

> **Nota**: En desarrollo, la redirección HTTPS está deshabilitada para evitar problemas con CORS.

### Poblar Base de Datos

Una vez que la API esté corriendo, pobla la base de datos con datos de ejemplo:

```bash
curl -X POST http://localhost:5000/api/seed
```

O usa Swagger UI en `http://localhost:5000` y ejecuta el endpoint `POST /api/seed`.

## 📚 Documentación

- **[Documentación Completa de la API](./API_DOCUMENTACION.md)** - Documentación detallada de todos los endpoints
- **[Guía de Ejecución Local](./GUIA_EJECUCION_LOCAL.md)** - Guía paso a paso para ejecutar localmente
- **[Conectar a MongoDB Atlas](./CONFIGURAR_MONGODB_ATLAS.md)** - Guía para conectarse y revisar la base de datos existente

### Swagger UI

Accede a la documentación interactiva cuando la API esté corriendo:
```
http://localhost:5000
```

## 🔌 Endpoints

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `GET` | `/api/properties` | Obtener todas las propiedades (con filtros opcionales) |
| `GET` | `/api/properties/{id}` | Obtener una propiedad por ID (incluye Owner, Images y Traces) |
| `POST` | `/api/seed` | Poblar base de datos con datos de ejemplo |
| `POST` | `/api/seed?repopulate=true` | Repoblar base de datos (elimina datos existentes) |
| `POST` | `/api/seed/add-more?count=100` | Agregar más propiedades sin eliminar las existentes |

### Parámetros de Filtro

| Parámetro | Tipo | Descripción |
|-----------|------|-------------|
| `name` | string | Búsqueda parcial por nombre (case-insensitive, sin acentos) |
| `address` | string | Búsqueda parcial por dirección (case-insensitive, sin acentos) |
| `minPrice` | decimal | Precio mínimo |
| `maxPrice` | decimal | Precio máximo |

> **Nota**: Las búsquedas por `name` y `address` ignoran acentos. Por ejemplo, buscar "medellin" encontrará propiedades con "Medellín", "MEDELLÍN", etc.

### Ejemplos

```bash
# Obtener todas las propiedades
curl http://localhost:5000/api/properties

# Filtrar por nombre (sin acentos)
curl "http://localhost:5000/api/properties?name=medellin"

# Filtrar por dirección (sin acentos)
curl "http://localhost:5000/api/properties?address=bogota"

# Filtrar por rango de precio
curl "http://localhost:5000/api/properties?minPrice=200000&maxPrice=300000"

# Agregar más propiedades
curl -X POST "http://localhost:5000/api/seed/add-more?count=50"
```

## 🧪 Testing

Ejecutar los tests unitarios:

```bash
cd tests/PropertyAPI.Tests
dotnet test
```

## 📁 Estructura del Proyecto

```
TerraNova-Backend/
├── src/
│   ├── PropertyAPI/              # Capa de presentación
│   │   ├── Controllers/          # Controladores de la API
│   │   ├── Scripts/              # Scripts de utilidad
│   │   └── Program.cs            # Punto de entrada
│   ├── PropertyAPI.Application/  # Capa de aplicación
│   │   ├── DTOs/                 # Data Transfer Objects
│   │   ├── Services/             # Servicios de negocio
│   │   └── Mappings/             # AutoMapper profiles
│   ├── PropertyAPI.Domain/       # Capa de dominio
│   │   ├── Entities/             # Entidades de dominio
│   │   └── Interfaces/            # Contratos
│   └── PropertyAPI.Infrastructure/ # Capa de infraestructura
│       ├── Data/                  # Contexto de MongoDB
│       ├── Repositories/          # Implementación de repositorios
│       └── Helpers/               # Helpers (TextNormalizer para búsquedas sin acentos)
├── tests/
│   └── PropertyAPI.Tests/        # Tests unitarios
├── API_DOCUMENTACION.md          # Documentación completa
├── GUIA_EJECUCION_LOCAL.md       # Guía de ejecución
├── README_API.md                 # Guía rápida
└── README.md                     # Este archivo
```

## 🔧 Configuración

### Variables de Entorno

Puedes configurar la conexión a MongoDB mediante:

1. **appsettings.json** (desarrollo)
2. **appsettings.Development.json** (desarrollo)
3. **appsettings.Production.json** (producción)
4. **Variables de entorno** (recomendado para producción)

### MongoDB Atlas

Si usas MongoDB Atlas, actualiza la cadena de conexión:

```json
{
  "ConnectionStrings": {
    "MongoDB": "mongodb+srv://usuario:password@cluster.mongodb.net/PropertyDB?retryWrites=true&w=majority"
  },
  "MongoDB": {
    "DatabaseName": "PropertyDB"
  }
}
```

Ver la guía completa: [CONFIGURAR_MONGODB_ATLAS.md](./CONFIGURAR_MONGODB_ATLAS.md)

### CORS

La API está configurada con CORS para permitir peticiones desde el frontend:

- **Desarrollo**: Permite todas las peticiones desde cualquier origen
- **Producción**: Deberías configurar orígenes específicos por seguridad

### HTTPS

- **Desarrollo**: HTTPS redirection está deshabilitada para evitar problemas con CORS
- **Producción**: HTTPS redirection está habilitada automáticamente

## 🤝 Contribuir

Las contribuciones son bienvenidas. Por favor:

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📝 Licencia

Este proyecto está bajo la Licencia MIT. Ver el archivo `LICENSE` para más detalles.

## 👥 Autores

- Tu Nombre - [@tu-usuario](https://github.com/tu-usuario)

## 🙏 Agradecimientos

- .NET Team
- MongoDB Team
- Comunidad de desarrolladores

## 📞 Soporte

Para soporte o preguntas:
- Abre un [Issue](https://github.com/tu-usuario/TerraNova-Backend/issues)
- Consulta la [Documentación](./API_DOCUMENTACION.md)

---

⭐ Si este proyecto te fue útil, considera darle una estrella en GitHub!

