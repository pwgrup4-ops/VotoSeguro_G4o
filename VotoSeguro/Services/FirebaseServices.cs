

using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;

namespace VotoSeguro.Services
{
    public class FirebaseServices
    {
        private readonly FirestoreDb _firestoreDb;
        private readonly string _projectId;

        public FirebaseServices(IConfiguration configuration)
        {
            // 1. Obtener el ProjectId de appsettings.json
            _projectId = configuration["Firebase:ProjectId"]
                         ?? throw new InvalidOperationException("Firebase ProjectID no configurado en appsettings.json");
            
            // 2. Inicializar Firebase App si no está inicializado
            if (FirebaseApp.DefaultInstance == null)
            {
                // =========================================================================
                // INICIO DE LA SOLUCIÓN: CARGA MANUAL DE CREDENCIALES
                // =========================================================================

                // *** REEMPLAZA ESTA RUTA CON LA RUTA ABSOLUTA A TU ARCHIVO JSON ***
                // La ruta debe tener doble barra invertida (\\)
                string credentialsPath = "C:\\Users\\GUIZEHN\\Desktop\\CARPETAS\\CARPETAS DE CLASES\\22. PROGRAMACION WEB\\SEMANA 9\\TAREAS\\VotoSeguro\\VotoSeguro\\Config\\firebase-credentials.json";

                if (!System.IO.File.Exists(credentialsPath))
                {
                    // Esto lanzará un error más claro si la ruta es incorrecta
                    throw new FileNotFoundException("El archivo de credenciales de Firebase no fue encontrado en la ruta especificada.", credentialsPath);
                }

                // Carga la credencial directamente desde el archivo.
                var credential = GoogleCredential.FromFile(credentialsPath);

                FirebaseApp.Create(new AppOptions
                {
                    Credential = credential,
                    ProjectId = _projectId
                });
                
                // =========================================================================
                // FIN DE LA SOLUCIÓN
                // =========================================================================
            }
            
            // 3. Crear instancia de Firestore
            _firestoreDb = FirestoreDb.Create(_projectId);    
        }
            
        public FirestoreDb GetFirestoreDb()
        {
            return _firestoreDb;
        }

        public CollectionReference GetCollection(string collectionName)
        {
            return _firestoreDb.Collection(collectionName);   
        }
    }
}