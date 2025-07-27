# Documentación - Diligencia de Proveedores - BackEnd .NET

Esta sección detalla la API RESTful que gestiona la información de proveedores y facilita los procesos de "screening".

# Visión General de la API

Esta API es el core del sistema de Diligencia de Proveedores. Permite a otras aplicaciones interactuar con los datos de proveedores de forma segura y estructurada, soportando operaciones CRUD (Crear, Leer, Actualizar, Eliminar) y procesos especializados de verificación.

## Tecnologías Usadas

- **ASP.NET Core 8+:** Framework principal para la construcción de la API.
- **Entity Framework Core:** ORM para la interacción con la base de datos SQL Server.
- **JWT Bearer Authentication:** Estándar para la autenticación basada en tokens.
- **Swagger/OpenAPI:** Para la documentación interactiva y el testeo de endpoints.
- **SQL Server:** Base de datos relacional para el almacenamiento de datos.

## URL Base de la API

Todas las solicitudes a la API deben comenzar con la siguiente URL base:

```
https://ey-diligencia-proveedores-web-bmehh2ghfnhmgnat.northcentralus-01.azurewebsites.net/api
```

## Documentación Interactiva (Swagger UI)

Puedes explorar y probar los endpoints de la API directamente usando la interfaz de Swagger UI. Es una herramienta invaluable para desarrolladores que consumen la API.

```
https://ey-diligencia-proveedores-web-bmehh2ghfnhmgnat.northcentralus-01.azurewebsites.net/swagger
```

## Convenciones Generales

Para asegurar la consistencia y facilitar el uso, la API sigue las siguientes convenciones:

- **Formato de Datos:** Todas las solicitudes y respuestas utilizan **JSON**.
- **Nombres de Propiedades:** Las propiedades en los objetos JSON están en formato `camelCase` (ej., `razonSocial`, `pageNumber`).
- **Códigos de Estado HTTP:** La API utiliza los códigos de estado HTTP estándar para indicar el resultado de una operación (ej., `200 OK`, `201 Created`, `400 Bad Request`, `401 Unauthorized`, `403 Forbidden`, `404 Not Found`, `500 Internal Server Error`).
- **Fechas y Horas:** Las fechas y horas se representan en formato ISO 8601 (ej., `AAAA-MM-DDTHH:mm:ss.sssZ`).
- **UUID/GUIDs:** Los identificadores únicos (IDs) son cadenas de texto con formato GUID (ej., `c1f8a7e0-1b2c-3d4e-5f6a-7b8c9d0e1f2a`).

# Arquitectura

El backend está diseñado siguiendo principios de **Arquitectura Limpia (Clean Architecture)**, lo que garantiza una clara separación de responsabilidades y una mayor facilidad para el mantenimiento, la prueba y la escalabilidad del sistema. Se compone de varias capas distintas, cada una con un propósito específico.

## Diagrama

![backend-net.drawio.png](backend-net.drawio.png)

## Capas

### Capa de Dominio

- **Propósito:** Esta es la capa central y el corazón de la aplicación. Contiene la **lógica de negocio pura** y las **entidades** de la aplicación, completamente independiente de cualquier detalle de implementación externa (bases de datos, frameworks, etc.). Es la capa más estable.
- **Contenido Principal:**
    - **Entidades (Entities):** Modelos de negocio fundamentales (ej., `Proveedor`).
    - **Interfaces de Repositorio (Repository Interfaces):** Define los contratos para la persistencia de datos (ej., `IProveedorRepository`), sin implementar cómo se almacenan o recuperan los datos.
    - **Value Objects:** Objetos que representan un concepto de dominio mediante sus atributos, sin identidad propia.
    - **Excepciones de Dominio:** Errores específicos de la lógica de negocio.
- **Dependencias:** **No depende de ninguna otra capa** del proyecto. Es el núcleo agnóstico a la infraestructura.

### Capa de Aplicación

- **Propósito:** Contiene la **lógica de negocio específica de la aplicación** (casos de uso) y orquesta las operaciones entre el Dominio y las capas externas. Traduce las intenciones del usuario en acciones de dominio y viceversa.
- **Contenido Principal:**
    - **Servicios de Aplicación (Application Services/Service Interfaces):** Implementan la lógica de negocio de alto nivel y coordinan las interacciones con el Dominio y la Infraestructura (ej., `IProveedorService`, `ProveedorService`).
    - **DTOs (Data Transfer Objects):** Objetos utilizados para la transferencia de datos entre la API y esta capa, así como entre esta capa y el Dominio. Esto desacopla los modelos de dominio de la forma en que los datos son expuestos o recibidos por la API (ej., `CreateProveedorDto`, `GetProveedorDto`, `PaginationParams`).
    - **Interfaces de Servicios Externos:** Definiciones de contratos para servicios que la aplicación consume (ej., `IScrapingApiClient`).
- **Dependencias:** **Solo depende de la Capa de Dominio**. Esto significa que solo puede interactuar con las entidades e interfaces definidas en el Dominio.

### Capa de Infraestructura

- **Propósito:** Esta capa es la encargada de **implementar los contratos** definidos en las capas de Dominio y Aplicación. Es donde se manejan todos los **detalles técnicos externos**, como la persistencia de datos, la comunicación con servicios externos y otros detalles de implementación.
- **Contenido Principal:**
    - **Implementaciones de Repositorio:** Lógica concreta para la interacción con la base de datos (ej., `ProveedorRepository` que usa Entity Framework Core para SQL Server).
    - **Configuración de Base de Datos:** `DbContext` (ej., `ApplicationDbContext`) para Entity Framework Core, migraciones y mapeo de entidades.
    - **Clientes de Servicios Externos:** Implementaciones concretas para interactuar con APIs de terceros (ej., la implementación de `IScrapingApiClient` que realiza llamadas HTTP).
    - **Gestión de Identidad y Autenticación:** Lógica de persistencia para usuarios, roles y gestión de seguridad.
- **Dependencias:** **Depende de la Capa de Dominio y de la Capa de Aplicación**. Necesita conocer las interfaces y entidades de Dominio para implementar los repositorios, y puede necesitar conocer interfaces de Aplicación para implementar clientes de servicios externos que la capa de Aplicación utiliza.

### Capa de Presentación/API

- **Propósito:** Esta es la **puerta de entrada externa** a la aplicación. Se encarga de la comunicación HTTP, el mapeo de rutas, la validación de modelos, la autenticación y autorización a nivel de endpoint, y la inyección de dependencias. **No contiene lógica de negocio.**
- **Contenido Principal:**
    - **Controladores (Controllers):** Clases que definen los endpoints RESTful (ej., `ProveedoresController`). Reciben las solicitudes HTTP, delegan la lógica a los Servicios de Aplicación y devuelven las respuestas HTTP.
    - **Configuración de Inicio:** `Program.cs` para configurar el *middleware*, la inyección de dependencias, CORS, autenticación JWT y Swagger.
    - **Filtros de Excepción:** Manejo global de errores HTTP (ej., `CustomExceptionFilter`).
    - **Modelos de Entrada/Salida:** Utiliza los DTOs definidos en la Capa de Aplicación.
- **Dependencias:** **Depende de la Capa de Aplicación, la Capa de Infraestructura y la Capa de Dominio**. Necesita acceder a los servicios de aplicación para ejecutar la lógica de negocio, a la infraestructura para configurar la inyección de dependencias de repositorios y clientes externos, y al Dominio para las entidades y los DTOs que se exponen.

## Principios Clave Aplicados

- **Separación de Responsabilidades:** Cada capa tiene un conjunto de responsabilidades bien definidas y limitadas.
- **Principio de Inversión de Dependencias (DIP):** Las capas de alto nivel (Dominio, Aplicación) no dependen de las capas de bajo nivel (Infraestructura, API) directamente en términos de implementación. En su lugar, las capas de alto nivel definen interfaces, y las capas de bajo nivel implementan esas interfaces. Esto hace que el código sea más flexible y desacoplado.
- **Independencia de Frameworks:** Las capas centrales (Dominio y Aplicación) son en gran medida independientes de tecnologías específicas (como ASP.NET Core o Entity Framework). Esto facilita el cambio de estas tecnologías en el futuro si fuera necesario sin afectar la lógica de negocio central.
- **Testabilidad:** La clara separación de lógica de negocio del acceso a datos y la interfaz de usuario hace que las capas centrales sean fácilmente testeables de forma unitaria y aislada.
- **Mantenibilidad y Escalabilidad:** Los cambios en una capa (ej., cambiar la base de datos o un servicio externo) tienen un impacto mínimo en otras capas, lo que agiliza el desarrollo y la adaptación a nuevas necesidades.

# Roles y Permisos

La API implementa un control de acceso basado en roles. Los usuarios pueden tener uno o más roles que determinan las operaciones que pueden realizar.

## User

Los usuarios con el rol `User` tienen permisos para las operaciones básicas de gestión de proveedores.

- **Permisos:**
    - Consultar la lista de proveedores (con paginación).
    - Obtener los detalles de un proveedor específico por ID o razón social.
    - Crear nuevos registros de proveedores.
    - Actualizar la información de proveedores existentes.

## Admin

Los usuarios con el rol `Admin` tienen acceso completo a todas las funcionalidades de la API, incluyendo operaciones sensibles.

- **Permisos:**
    - Todos los permisos del rol `User`.
    - Eliminar registros de proveedores.
    - Iniciar procesos de "screening" para proveedores.

# Autenticación

La API utiliza tokens JWT (JSON Web Tokens) para la autenticación y autorización. Todas las solicitudes a los endpoints protegidos deben incluir un token JWT válido en el encabezado `Authorization`.

## Generar token JWT

Solo las personas que accedan al sistema pueden crear un usuario. Esto bajo la premisa de que ningún trabajador puede crear su cuenta, mas siempre se le asignan una.

### Endpoint

```
POST /api/auth/login
```

### Body de la solicitud

```json
{
"username": "nombre_de_usuario",
"password": "tu_contraseña_secreta"
}
```

## Obtener un Token JWT

Para obtener un token, debes realizar una solicitud `POST` al endpoint de login con tus credenciales.

### **Endpoint:**

`POST /api/auth/login`

### **Body de Solicitud (JSON):**

```json
{
  "username": "nombre_de_usuario",
  "password": "tu_contraseña_secreta"
}
```

### **Ejemplo de Respuesta Exitosa:**

```json
{
  "userId": "b0fba25c-da2f-42d9-a57a-181398daa01f",
  "email": "user@example.com",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb29wLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJ1c2VyQGV4YW1wbGUuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMsImlhdCI6MTY3MjIzOTAyMiwiZXhwIjoxNzUzNjM0NjA5LCJpc3MiOiJodHRwczovL2V5LWRpbGlnZW5jaWEtcHJvdmVlZG9yZXMtd2ViLWJtZWhoMmdoZm5obWduYXQubm9ydGhjZW50cmFsdXMtMDEuYXp1cmV3ZWJzaXRlcy5uZXQiLCJhdWQiOiJodHRwczovL2V5LWRpbGlnZW5jaWEtcHJvdmVlZG9yZXMtd2ViLWJtZWhoMmdoZm5obWduYXQubm9ydGhjZW50cmFsdXMtMDEuYXp1cmV3ZWJzaXRlcy5uZXQifQ.MVoLCRwDwQFkE80iXJn5zeYtLuULIqj-cxSotCkaAa4",
  "roles": [
    "User"
  ]
}
```

- **`userId`**: El ID único del usuario autenticado.
- **`email`**: El correo electrónico del usuario.
- **`token`**: El JWT que usarás en tus futuras solicitudes.
- **`roles`**: Un arreglo de los roles asignados al usuario (ej., `"User"`, `"Admin"`).

## Usar el Token JWT en Swagger

Una vez que tengas el token JWT, debes incluirlo en el encabezado `Authorization` de **todas las solicitudes posteriores** a endpoints protegidos.

### Formato

```json
Authorization: Bearer <TU_TOKEN_JWT_AQUÍ>
```

# Endpoints de la API

Gestión completa de la información de los proveedores registrados en el sistema.

### GET /api/proveedores

- **Propósito:** Recuperar una lista paginada de todos los proveedores disponibles.
- **Roles Requeridos:** `User` o `Admin`.
- **Parámetros de Consulta (Query Parameters):**
    - `pageNumber` (integer, opcional):
        - **Descripción:** El número de página a recuperar. Las páginas comienzan desde 1.
        - **Ejemplo:** `1`
    - `pageSize` (integer, opcional):
        - **Descripción:** La cantidad de elementos por página.
        - **Ejemplo:** `10`
- **Ejemplo de Solicitud:**
    
    ```
    GET https://ey-diligencia-proveedores-web-bmehh2ghfnhmgnat.northcentralus-01.azurewebsites.net/api/proveedores?pageNumber=1&pageSize=5
    ```
    
- **Respuestas:**
    - **`200 OK` (Respuesta Exitosa):** Devuelve un objeto paginado con la lista de proveedores.
        
        ```json
        {
          "items": [
            {
              "id": "c1f8a7e0-1b2c-3d4e-5f6a-7b8c9d0e1f2a",
              "razonSocial": "Empresa Ejemplo S.A.C.",
              "ruc": "20123456789",
              "direccion": "Av. Los Rosales 123",
              "fechaCreacion": "2024-07-27T10:00:00Z"
            }
            // ... más objetos GetProveedorDto
          ],
          "pageNumber": 1,
          "pageSize": 5,
          "totalPages": 2,
          "totalCount": 10,
          "hasPreviousPage": false,
          "hasNextPage": true
        }
        ```
        
    - **`401 Unauthorized`:** Si no se proporciona un token JWT válido o está ausente.
    - **`403 Forbidden`:** Si el token JWT es válido pero el usuario no tiene el rol `User` o `Admin`.

---

### GET /api/proveedores/{id}

- **Propósito:** Obtener los detalles de un proveedor específico utilizando su ID único.
- **Roles Requeridos:** `User` o `Admin`.
- **Parámetros de Ruta (Path Parameters):**
    - `id` (GUID, requerido):
        - **Descripción:** El identificador único (GUID) del proveedor.
        - **Ejemplo:** `c1f8a7e0-1b2c-3d4e-5f6a-7b8c9d0e1f2a`
- **Ejemplo de Solicitud:**
    
    ```
    GET https://ey-diligencia-proveedores-web-bmehh2ghfnhmgnat.northcentralus-01.azurewebsites.net/api/proveedores/c1f8a7e0-1b2c-3d4e-5f6a-7b8c9d0e1f2a
    ```
    
- **Respuestas:**
    - **`200 OK` (Respuesta Exitosa):** Devuelve el objeto `GetProveedorDto` del proveedor solicitado.
        
        ```json
        {
          "id": "c1f8a7e0-1b2c-3d4e-5f6a-7b8c9d0e1f2a",
          "razonSocial": "Empresa Ejemplo S.A.C.",
          "ruc": "20123456789",
          "direccion": "Av. Los Rosales 123",
          "fechaCreacion": "2024-07-27T10:00:00Z"
        }
        ```
        
    - **`401 Unauthorized`:** Si no se proporciona un token JWT válido.
    - **`403 Forbidden`:** Si el usuario no tiene el rol `User` o `Admin`.
    - **`404 Not Found`:** Si no se encuentra un proveedor con el ID proporcionado.

---

### GET /api/proveedores/name?razonSocial={razonSocial}

- **Propósito:** Buscar un proveedor por su razón social.
- **Roles Requeridos:** `User` o `Admin`.
- **Parámetros de Consulta (Query Parameters):**
    - `razonSocial` (string, requerido):
        - **Descripción:** La razón social completa o parcial del proveedor a buscar.
        - **Ejemplo:** `Empresa Ejemplo S.A.C.` (usar `%20` para espacios en URL)
- **Ejemplo de Solicitud:**
    
    ```
    GET https://ey-diligencia-proveedores-web-bmehh2ghfnhmgnat.northcentralus-01.azurewebsites.net/api/proveedores/name?razonSocial=Empresa%20Ejemplo%20S.A.C.
    ```
    
- **Respuestas:**
    - **`200 OK` (Respuesta Exitosa):** Devuelve el objeto `GetProveedorDto` del proveedor encontrado.
        
        ```json
        {
          "id": "c1f8a7e0-1b2c-3d4e-5f6a-7b8c9d0e1f2a",
          "razonSocial": "Empresa Ejemplo S.A.C.",
          "ruc": "20123456789",
          "direccion": "Av. Los Rosales 123",
          "fechaCreacion": "2024-07-27T10:00:00Z"
        }
        ```
        
    - **`400 Bad Request`:** Si el parámetro `razonSocial` está vacío o no es válido.
    - **`401 Unauthorized`:** Si no se proporciona un token JWT válido.
    - **`403 Forbidden`:** Si el usuario no tiene el rol `User` o `Admin`.
    - **`404 Not Found`:** Si no se encuentra un proveedor con la razón social proporcionada.

---

### POST /api/proveedores

- **Propósito:** Crear un nuevo registro de proveedor en el sistema.
- **Roles Requeridos:** `User` o `Admin`.
- **Body de Solicitud (JSON):** Se espera un objeto `CreateProveedorDto`.
    
    ```json
    {
      "razonSocial": "Nuevo Proveedor XYZ S.A.C.",
      "ruc": "20123456789",
      "direccion": "Calle Ficticia 456",
      "telefono": "+51987654321",
      "correoElectronico": "contacto@nuevoproveedor.com"
    }
    ```
    
- **Ejemplo de Solicitud:**
    
    ```
    POST https://ey-diligencia-proveedores-web-bmehh2ghfnhmgnat.northcentralus-01.azurewebsites.net/api/proveedores
    ```
    
- **Respuestas:**
    - **`201 Created` (Proveedor Creado):** Devuelve el objeto `GetProveedorDto` del proveedor recién creado, incluyendo su ID generado.
        
        ```json
        {
          "id": "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d",
          "razonSocial": "Nuevo Proveedor XYZ S.A.C.",
          "ruc": "20123456789",
          "direccion": "Calle Ficticia 456",
          "telefono": "+51987654321",
          "correoElectronico": "contacto@nuevoproveedor.com",
          "fechaCreacion": "2025-07-27T11:00:00Z"
        }
        ```
        
    - **`400 Bad Request`:** Si los datos proporcionados en el body de la solicitud son inválidos (ej., campos requeridos faltantes, formatos incorrectos).
    - **`401 Unauthorized`:** Si no se proporciona un token JWT válido.
    - **`403 Forbidden`:** Si el usuario no tiene el rol `User` o `Admin`.

---

### PUT /api/proveedores/{id}

- **Propósito:** Actualizar la información de un proveedor existente por su ID.
- **Roles Requeridos:** `User` o `Admin`.
- **Parámetros de Ruta (Path Parameters):**
    - `id` (GUID, requerido):
        - **Descripción:** El ID único del proveedor a actualizar.
        - **Ejemplo:** `c1f8a7e0-1b2c-3d4e-5f6a-7b8c9d0e1f2a`
- **Body de Solicitud (JSON):** Se espera un objeto `UpdateProveedorDto`. Proporciona solo los campos que deseas actualizar.
    
    ```json
    {
      "razonSocial": "Empresa Ejemplo Actualizada",
      "direccion": "Nueva Dirección Principal 789"
    }
    ```
    
- **Ejemplo de Solicitud:**
    
    ```
    PUT https://ey-diligencia-proveedores-web-bmehh2ghfnhmgnat.northcentralus-01.azurewebsites.net/api/proveedores/c1f8a7e0-1b2c-3d4e-5f6a-7b8c9d0e1f2a
    ```
    
- **Respuestas:**
    - **`204 No Content` (Actualización Exitosa):** La operación fue exitosa y no hay contenido que devolver.
    - **`400 Bad Request`:** Si los datos proporcionados en el body de la solicitud son inválidos.
    - **`401 Unauthorized`:** Si no se proporciona un token JWT válido.
    - **`403 Forbidden`:** Si el usuario no tiene el rol `User` o `Admin`.
    - **`404 Not Found`:** Si no se encuentra un proveedor con el ID proporcionado.

---

### DELETE /api/proveedores/{id}

- **Propósito:** Eliminar un proveedor existente por su ID. **¡Esta operación es irreversible!**
- **Roles Requeridos:** `Admin` (solo usuarios con rol de administrador).
- **Parámetros de Ruta (Path Parameters):**
    - `id` (GUID, requerido):
        - **Descripción:** El ID único del proveedor a eliminar.
        - **Ejemplo:** `c1f8a7e0-1b2c-3d4e-5f6a-7b8c9d0e1f2a`
- **Ejemplo de Solicitud:**
    
    ```
    DELETE https://ey-diligencia-proveedores-web-bmehh2ghfnhmgnat.northcentralus-01.azurewebsites.net/api/proveedores/c1f8a7e0-1b2c-3d4e-5f6a-7b8c9d0e1f2a
    ```
    
- **Respuestas:**
    - **`204 No Content` (Eliminación Exitosa):** La operación fue exitosa y no hay contenido que devolver.
    - **`401 Unauthorized`:** Si no se proporciona un token JWT válido.
    - **`403 Forbidden`:** Si el usuario no tiene el rol `Admin`.
    - **`404 Not Found`:** Si no se encuentra un proveedor con el ID proporcionado.

---

### POST /api/proveedores/{id}/screening

- **Propósito:** Iniciar un proceso de "screening" (rastreo o verificación) para un proveedor específico.
- **Roles Requeridos:** `Admin` (solo usuarios con rol de administrador).
- **Parámetros de Ruta (Path Parameters):**
    - `id` (GUID, requerido):
        - **Descripción:** El ID único del proveedor para el cual se realizará el screening.
        - **Ejemplo:** `c1f8a7e0-1b2c-3d4e-5f6a-7b8c9d0e1f2a`
- **Body de Solicitud (JSON):**
    - Si `ScreeningRequestDto` contiene campos, descríbelos aquí. Si es un body vacío (`{}`), indícalo.
    - *Ejemplo:*
        
        ```json
        {
          "tipoScreening": "completo",
          "fuentes": ["registros_publicos", "noticias"]
        }
        ```
        
- **Ejemplo de Solicitud:**
    
    ```
    
    ```
    
- **Respuestas:**
    - **`200 OK` (Screening Iniciado):** Devuelve información sobre el inicio del proceso de screening.
        
        ```json
        {
          "status": "Iniciado",
          "screeningId": "abc123def456-screening-process-id",
          "details": "El proceso de screening ha comenzado exitosamente. Los resultados estarán disponibles pronto."
        }
        ```
        
    - **`400 Bad Request`:** Si el body de la solicitud es inválido.
    - **`401 Unauthorized`:** Si no se proporciona un token JWT válido.
    - **`403 Forbidden`:** Si el usuario no tiene el rol `Admin`.
    - **`404 Not Found`:** Si no se encuentra un proveedor con el ID proporcionado.

# Base de Datos y Persistencia

La API utiliza **SQL Server** con **Entity Framework Core (EF Core)** para la gestión de datos.

## Configuración y Conexión

- **DB:** SQL Server.
- **ORM:** Entity Framework Core.
- **Cadena de Conexión:** `DefaultConnection`.
    - **Localmente:** Definida en `appsettings.Development.json` (para la DB local o de Azure, si la apuntas allí).
    - **Azure App Service:** Configurada en "Cadenas de conexión" del App Service, sobrescribiendo el `appsettings.json` desplegado.
- *Ejemplo de `appsettings.json`*
    
    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Server=tcp:yourserver.database.windows.net,1433;Initial Catalog=yourdatabase;Persist Security Info=False;User ID=youruser;Password=yourpassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
      }
    }
    ```
    
- **DbContext:** La clase `ApplicationDbContext` hereda de `DbContext` de Entity Framework Core y es el punto principal para interactuar con la base de datos. Define los `DbSet` para cada entidad que se mapea a una tabla en la base de datos (ej., `DbSet<Proveedor> Proveedores`).

## Migraciones de Entity Framework Core

### Local

Crear una migración:

```
dotnet ef migrations add [NombreDeLaMigracion] --project DiligenciaProveedores.Infrastructure/DiligenciaProveedores.Infrastructure.csproj --startup-project DiligenciaProveedores.Api/DiligenciaProveedores.Api.csproj
```

Aplicar una migración:

```
dotnet ef database update --project DiligenciaProveedores.Infrastructure/DiligenciaProveedores.Infrastructure.csproj --startup-project DiligenciaProveedores.Api/DiligenciaProveedores.Api.csproj
```

### Azure

- Observación: La IP debe estár en el firewall de Azure SQL y tu `appsettings.Development.json` apunta a la DB de Azure.
- Ejecutar `dotnet ef database update` localmente.

## Interacción con Datos (Repositorios)

La interacción con la DB se realiza a través del patrón Repositorio.

- **Interfaces de Repositorio (Capa `Domain`):** Definen los contratos de acceso a datos (ej., `IProveedorRepository`).
- **Implementaciones de Repositorio (Capa `Infrastructure`):** Contienen la lógica para usar `ApplicationDbContext` de EF Core.
