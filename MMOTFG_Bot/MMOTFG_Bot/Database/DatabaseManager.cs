using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace MMOTFG_Bot
{
	static class DatabaseManager
	{
		private static FirestoreDb db;
		private static DocumentReference docRefBase;
		private static CollectionReference collRefBase;

		private static string collectionName = "Estructura";
		private static string documentName = "pruebas";
		private static string subCollectionName = "pruebasPlayers";


		public static void Init()
		{
			Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "assets/private/firebase-admin.json");
			db = FirestoreDb.Create("mmotfg-database");
			docRefBase = db.Collection(collectionName).Document(documentName);
			collRefBase = docRefBase.Collection(subCollectionName);

			//docRefBase = db.Collection("Estructura").Document("players").Collection("PlayerList").Document("Pedro");
		}

		public static async Task<bool> addDocumentToCollection(Dictionary<string, object> document, string documentID, string collection)
		{
			DocumentReference docRef = db.Collection(collection).Document(documentID);

			try
			{
				await docRef.CreateAsync(document);
				return true;
			}

			catch
			{
				Console.WriteLine("Document {0} already exists in collection {1}. Use modifyDocument instead.", documentID, collection);
				return false;
			}

		}

		public static async Task modifyDocumentFromCollection(Dictionary<string, object> updates, string documentID, string collection)
		{
			CollectionReference colRef = db.Collection(collection);

			if (colRef == null)
			{
				Console.WriteLine("Collection {0} doesnt exist, remember its case sentitive.", collection);
				return;
			}

			DocumentReference docRef = colRef.Document(documentID);

			if (docRef == null)
			{
				Console.WriteLine("Document {0} doesnt exist in collection {1}, remember its case sentitive.", documentID, collection);
				return;
			}

			await docRef.UpdateAsync(updates);

		}

		public static async Task<Dictionary<string, object>> getDocument(string documentID, string collection)
		{
			CollectionReference colRef = db.Collection(collection);

			if (colRef == null)
			{
				Console.WriteLine("Collection {0} doesnt exist, remember its case sentitive.", collection);
				return null;
			}

			DocumentReference docRef = colRef.Document(documentID);

			if (docRef == null)
			{
				Console.WriteLine("Document {0} doesnt exist in collection {1}, remember its case sentitive.", documentID, collection);
				return null;
			}

			DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();

			if (docSnap.Exists)
			{
				Dictionary<string, object> ret = docSnap.ToDictionary();
				return ret;
			}
			else
			{
				Console.WriteLine("Error while retrieving the snapshot of document {0} from collection {1}.", docRef, collection);
				return null;
			}
		}


		public static async Task<Dictionary<string, object>[]> getDocumentsByFieldValue(string field, object value, string collection)
		{
			CollectionReference colRef = db.Collection(collection);

			if (colRef == null)
			{
				Console.WriteLine("Collection {0} doesnt exist, remember its case sentitive.", collection);
				return null;
			}

			QuerySnapshot querySnap = await colRef.WhereEqualTo(field, value).GetSnapshotAsync();

			if (colRef == null) Console.WriteLine("A document with {0} : {1} doesnt exist in collection {2}.", field, value.ToString(), collection);			

			Dictionary<string, object>[] ret = new Dictionary<string, object>[querySnap.Count];

			int i = 0;

			foreach (DocumentSnapshot documentSnapshot in querySnap.Documents)
			{
				Dictionary<string, object> temp = documentSnapshot.ToDictionary();
				ret[i] = temp;
				i++;
			}

			return ret;
		}

		public static async Task<Dictionary<string, object>> getDocumentByUniqueValue(string field, object value, string collection)
		{
			CollectionReference colRef = db.Collection(collection);

			if (colRef == null)
			{
				Console.WriteLine("Collection {0} doesnt exist, remember its case sentitive.", collection);
				return null;
			}

			QuerySnapshot querySnap = await colRef.WhereEqualTo(field, value).GetSnapshotAsync();

			if (colRef == null) { 
				Console.WriteLine("A document with {0} : {1} doesnt exist in collection {2}.", field, value.ToString(), collection);
				return null;
			}

			if(querySnap.Documents.Count > 1) {
				Console.WriteLine("There are multiple ({0}) documents with key value pair {1} : {2}.", querySnap.Documents.Count , field, value.ToString(), collection);
				return null;
			}

			Dictionary<string, object> ret = querySnap.Documents[0].ToDictionary();
			
			return ret;
		}
	}
}
