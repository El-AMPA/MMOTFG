using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using Newtonsoft.Json.Linq;

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
			string jsontext = System.IO.File.ReadAllText("assets/private/firebase-admin.json");
			var dbInfo = JObject.Parse(jsontext);
			db = FirestoreDb.Create(dbInfo["project_id"].Value<String>());
			docRefBase = db.Collection(collectionName).Document(documentName);
			collRefBase = docRefBase.Collection(subCollectionName);

			//docRefBase = db.Collection("Estructura").Document("players").Collection("PlayerList").Document("Pedro");
		}

		/// <summary>
		/// Adds a document to an already existing collection
		/// </summary>
		/// <param name="document">Dictionary containing the info of the document to be added</param>
		/// <param name="documentID">Unique ID of the document to be added to the collectiong. <b>MUST BE UNIQUE</b>. Also what you will use to retrieve it later</param>
		/// <param name="collection">Name of the collection to which you are adding the document, should be in <see cref="DbConstants"/></param>
		/// <returns>True if the element could be added to the collection, False otherwise</returns>
		public static async Task<bool> AddDocumentToCollection(Dictionary<string, object> document, string documentID, string collection)
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

		/// <summary>
		/// Changes or adds fields to an already existing document.
		/// If a field already exists in the database, it will be updated.
		/// If it doesn't exist, it will be created.
		/// </summary>
		/// <param name="updates">Dictionary containing the info of changes to be made</param>
		/// <param name="documentID">ID of the document to be modified</param>
		/// <param name="collection">Name of the collection that has the document, should be in <see cref="DbConstants"/></param>
		public static async Task ModifyDocumentFromCollection(Dictionary<string, object> updates, string documentID, string collection)
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

		/// <summary>
		/// Gets the specified document from the specified collection
		/// </summary>
		/// <param name="documentID">ID of the document to be retrieved</param>
		/// <param name="collection">Name of the collection to which you are adding the document, should be in <see cref="DbConstants"/></param>
		/// <returns>A dictionary containing the data from the given document, null if there are any errors</returns>
		public static async Task<Dictionary<string, object>> GetDocument(string documentID, string collection)
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


		/// <summary>
		/// Gets the documents that have the given field-value pair from the specified collection 
		/// </summary>
		/// <param name="field">Field that will be searched, should be in <see cref="DbConstants"/></param>
		/// <param name="value">Value to match</param>
		/// <param name="collection">Name of the collection to search, should be in <see cref="DbConstants"/></param>
		/// <returns>An array of dictionaries with the data of all the documents that match, null if there are any errors</returns>
		/// 
		public static async Task<Dictionary<string, object>[]> GetDocumentsByFieldValue(string field, object value, string collection)
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

		/// <summary>
		/// Gets the document that have the given field-value pair from the specified collection
		/// </summary>
		/// <param name="field">Field that will be searched, should be in <see cref="DbConstants"/>)</param>
		/// <param name="value">Value to match</param>
		/// <param name="collection">Name of the collection to search, should be in <see cref="DbConstants"/></param>
		/// <returns>A dictionary containing the data from the given document, null if there are more than one or there are any errors</returns>
		public static async Task<Dictionary<string, object>> GetDocumentByUniqueValue(string field, object value, string collection)
		{
			CollectionReference colRef = db.Collection(collection);

			if (colRef == null)
			{
				Console.WriteLine("Collection {0} doesnt exist, remember its case sentitive.", collection);
				return null;
			}

			QuerySnapshot querySnap = await colRef.WhereEqualTo(field, value).GetSnapshotAsync();

			if (querySnap.Documents.Count <= 0)
			{
				Console.WriteLine("A document with {0} : {1} doesnt exist in collection {2}.", field, value.ToString(), collection);
				return null;
			}

			if (querySnap.Documents.Count > 1)
			{
				Console.WriteLine("There are multiple ({0}) documents with key value pair {1} : {2}.", querySnap.Documents.Count, field, value.ToString(), collection);
				return null;
			}

			Dictionary<string, object> ret = querySnap.Documents[0].ToDictionary();

			return ret;
		}

		/// <summary>
		/// Erases the document with the given id from the database
		/// </summary>
		/// <param name="documentId">ID of the document to be deleted</param>
		/// <param name="collection">Name of the collection to search, should be in <see cref="DbConstants"/></param>
		/// <returns>True if the deletios was successful, false otherwise</returns>
		public static async Task<bool> DeleteDocumentById(string documentId, string collection)
		{
			CollectionReference colRef = db.Collection(collection);

			if (colRef == null)
			{
				Console.WriteLine("Collection {0} doesnt exist, remember its case sentitive.", collection);
				return false;
			}

			DocumentReference docRef = colRef.Document(documentId);

			if (docRef == null)
			{
				Console.WriteLine("Document {0} doesnt exist in collection {1}, remember its case sentitive.", documentId, collection);
				return false;
			}

			await docRef.DeleteAsync();

			return true;
		}

		/// <summary>
		/// Retrieves a single field from the given document
		/// </summary>
		/// <param name="fieldName">name of the field, should be in <see cref="DbConstants"/></param>
		/// <param name="documentId">ID of the document with the given field</param>
		/// <param name="collection">Name of the collection to search, should be in <see cref="DbConstants"/></param>
		/// <returns>True if the deletios was successful, false otherwise</returns>
		public static async Task<object> GetFieldFromDocument(string fieldName, string documentId, string collection)
		{
			CollectionReference colRef = db.Collection(collection);

			if (colRef == null)
			{
				Console.WriteLine("Collection {0} doesnt exist, remember its case sentitive.", collection);
				return null;
			}

			DocumentReference docRef = colRef.Document(documentId);

			if (docRef == null)
			{
				Console.WriteLine("Document {0} doesnt exist in collection {1}, remember its case sentitive.", documentId, collection);
				return null;
			}

			DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();

			Dictionary<string, object> playerDict = docSnap.ToDictionary();

			object ret = new object();

			if (!playerDict.TryGetValue(fieldName, out ret))
			{
				ret = null;
			}

			return ret;
		}

		/// <summary>
		/// Updates a single field from the given document
		/// </summary>
		/// <param name="fieldName">name of the field, should be in <see cref="DbConstants"/></param>
		/// <param name="newValue">value that the given field will take</param>
		/// <param name="documentId">ID of the document with the given field</param>
		/// <param name="collection">Name of the collection to search, should be in <see cref="DbConstants"/></param>
		/// <returns>True if the deletios was successful, false otherwise</returns>
		public static async Task ModifyFieldOfDocument(string fieldName, object newValue, string documentId, string collection)
		{
			CollectionReference colRef = db.Collection(collection);

			if (colRef == null)
			{
				Console.WriteLine("Collection {0} doesnt exist, remember its case sentitive.", collection);
				return;
			}

			DocumentReference docRef = colRef.Document(documentId);

			if (docRef == null)
			{
				Console.WriteLine("Document {0} doesnt exist in collection {1}, remember its case sentitive.", documentId, collection);
				return;
			}

			DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();

			Dictionary<string, object> playerDict = docSnap.ToDictionary();

			if (!playerDict.TryGetValue(fieldName, out _))
			{
				Console.WriteLine("Document {0} doesnt have field {1}, you cant modify it", documentId, fieldName);
				return;
			}

			Dictionary<string, object> update = new Dictionary<string, object> { { fieldName, newValue } };

			await docRef.UpdateAsync(update);
		}


		/// <summary>
		/// Returns whether or not a document is in a collection
		/// </summary>
		/// <param name="documentId">Name of the document to search for)</param>
		/// <param name="collection">Name of the collection to search, should be in <see cref="DbConstants"/></param>
		/// <returns>True if the document is found, false otherwise</returns>
		public static async Task<bool> IsDocumentInCollection(string documentId, string collection)
		{
			CollectionReference colRef = db.Collection(collection);

			if (colRef == null)
			{
				Console.WriteLine("Collection {0} doesnt exist, remember its case sentitive.", collection);
				return false;
			}


			DocumentReference docRef = colRef.Document(documentId);

			DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();

			return docSnap.Exists;

		}

		public static async Task DeleteCollection(string collection)
		{
			CollectionReference colRef = db.Collection(collection);

			if (colRef == null)
			{
				Console.WriteLine("Collection {0} doesnt exist, remember its case sentitive.", collection);
				return;
			}

			QuerySnapshot snapshot = await colRef.GetSnapshotAsync();
			IReadOnlyList<DocumentSnapshot> documents = snapshot.Documents;
			while (documents.Count > 0)
			{
				foreach (DocumentSnapshot document in documents)
				{
					await document.Reference.DeleteAsync();
				}
				snapshot = await colRef.GetSnapshotAsync();
				documents = snapshot.Documents;
			}
			Console.WriteLine("Finished deleting all documents from the collection.");
		}
	}
}
