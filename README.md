# QuestionsAnswersApp
Autor: Ricardo Romeo Ramos Recinos  romeo4ramos@gmail.com

Esta es una aplicación de preguntas y respuestas desarrollada en C# utilizando .NET, que consta de dos proyectos principales:

- **QuestionsAnswers.API**: Este proyecto tiene la API REST que se encarga de acceder y persistir los datos en la base de datos QuestionsAnswersDB, 
							para ello hace uso de procedimientos almacenados para las tablas de Pregunta(Answer), Respuesta(Answer) y usuario(UserQA),
							luego se encarga recibir las peticiones de la aplicacion web QuestionsAnswers.Web y devolver las respuestas requeridas de esta ultima

- **QuestionsAnswers.Web**: Es la aplicación web donde los usuarios pueden registrarse, iniciar sesión, hacer preguntas y respuestas. Contiene toda las
							pantallas necesarias para que el usuario pueda crear, responder y cerrar preguntas, esta se encarga de hacer la peticiones de datos
							a la API REST anterior y luego manejar las respuestas y presentar los resultados al usuario.


## Notas importantes a considerar
- El proyecto lo desarrolle utilizando codigo completamente en ingles, ya que me resulta más familiar de manejar por mi experiecia previa, pero puedo perfectamente
  trabajarlo en español. Para la pantallas que verá el usuario si opté por presentarlo en español, para que sea más directo de relacionar con el documento de la prueba técnica.
- Cree una única solución en Visual Studio para ambos proyectos llamada QuestionsAnswersApp y al momento de probarla se debe configurar para que corra ambos proyectos a la vez.
- La base de datos que utilicé es SQL server 2022; en mi caso utilicé Docker con una imagen de esta para construir la aplicación. Además cree un usuario propio para acceder
  a la base de datos QuestionsAnswersDB, puede revisar appsettings.Development.json del proyecto 


  

## Tecnologías utilizadas

- **C#**: Lenguaje de programación principal.
- **ASP.NET Core**: Framework para la creación de la API REST.
- **SQL Server**: Sistema de gestión de base de datos.
- **Microsoft.Data.SqlClient**: Librería utilizada para realizar operaciones directas con SQL Server mediante ADO.NET. Se utiliza exclusivamente para ejecutar procedimientos 
                                almacenados y manejar las conexiones a la base de datos.
- **Razor Pages**: Utilizado en la aplicación web para manejar las vistas.

## Puntos completados de la prueba tecnica:
- Como dueño de la aplicación quiero que esta cuente con un registro de usuarios en el que se solicite usuario y contraseña. Al momento del registro se debe validar que 
  el usuario no exista y que ninguno de los campos esté vacío.(COMPLETADO)

- Como usuario deseo poder iniciar sesión con mi usuario y contraseña registrados.(COMPLETADO)

- Como usuario deseo poder ver en la página principal el listado de preguntas hechas por mí y por los demás usuarios luego de hacer login, 
  ordenadas de la más reciente a la más antigua.(COMPLETADO)

- Como usuario deseo poder ver un botón en la página principal que me permita “hacer una pregunta”.(COMPLETADO)
-
- Como usuario deseo poder responder preguntas hechas por los demás usuarios.(COMPLETADO)

- Como usuario deseo ver las respuestas que los demás usuarios han dado a una pregunta.(COMPLETADO)

- Como usuario deseo poder cerrar una pregunta que yo haya hecho, esto hará que ya nadie pueda dar respuestas a dicha pregunta.(COMPLETADO)


## Como probar la aplicacion (Visual Studio 2022)
- Primero descargar la solucion en la carpeta deseada:
  git clone https://github.com/romeo4r/QuestionsAnswersApp.git
- Luego abrir la carpeta de la solucion y correr el archivo CreateDB.sql en SQL Server 
- Abrir la solucion en Visual Studio: QuestionsAnswersApp.sln
- Luego configurarlo para que corra los 2 proyectos a la vez:
  -- Clic Derecho en la raiz de la solucion QuestionsAnswersApp y luego propiedades
  -- Luego en la seccion de Startup Project seleccionar Multiple startup projects y configurar los Action de cada uno en Start

- Por ultimo correr el proyecto y probarlo!!

