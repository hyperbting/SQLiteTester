using System;
using System.Collections;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SQLiteUSer : MonoBehaviour
{
    private string dbPath;

    public Text debugBoard;
    
    private void Start() {
      dbPath = "URI=file:" + Application.persistentDataPath + "/exampleDatabase.db";
      CreateSchema();

      RemovePlayerScores("GG Meade");
      RemovePlayerScores("US Grant");
      RemovePlayerScores("GB McClellan");

      InvokeRepeating("PeroidInsertion" ,1, 1);
    }

    private void PeroidInsertion()
    {
      debugBoard.text = $"{DateTime.Now}\n" + debugBoard.text;
      var max = DateTime.Now.Millisecond;
      InsertScore("GG Meade", Random.Range(1,max));
      InsertScore("US Grant", Random.Range(1,max));
      InsertScore("GB McClellan", Random.Range(1,max));
      GetHighScores(10);
    }

    public void CreateSchema() {
      using (var conn = new SqliteConnection(dbPath)) {
        conn.Open();
        using (var cmd = conn.CreateCommand()) {
          cmd.CommandType = CommandType.Text;
          cmd.CommandText = "CREATE TABLE IF NOT EXISTS 'high_score' ( " +
                            "  'id' INTEGER PRIMARY KEY, " +
                            "  'name' TEXT NOT NULL, " +
                            "  'score' INTEGER NOT NULL" +
                            ");";

          var result = cmd.ExecuteNonQuery();
          Debug.Log("create schema: " + result);
          debugBoard.text = $"create schema: {result} \n" + debugBoard.text;
        }
      }
    }

    public void RemovePlayerScores(string highScoreName) {
      using (var conn = new SqliteConnection(dbPath)) {
        conn.Open();
        using (var cmd = conn.CreateCommand()) {
          cmd.CommandType = CommandType.Text;
          cmd.CommandText = "DELETE FROM high_score WHERE name = @Name;";

          cmd.Parameters.Add(new SqliteParameter {
            ParameterName = "Name",
            Value = highScoreName
          });

          var result = cmd.ExecuteNonQuery();
          Debug.Log($"remove player: {highScoreName}" + result);
          //debugBoard.text = $"insert score: {result} \n" + debugBoard.text;
        }
      }
    }
    
    public void InsertScore(string highScoreName, int score) {
      using (var conn = new SqliteConnection(dbPath)) {
        conn.Open();
        using (var cmd = conn.CreateCommand()) {
          cmd.CommandType = CommandType.Text;
          cmd.CommandText = "INSERT INTO high_score (name, score) " +
                            "VALUES (@Name, @Score);";

          cmd.Parameters.Add(new SqliteParameter {
            ParameterName = "Name",
            Value = highScoreName
          });

          cmd.Parameters.Add(new SqliteParameter {
            ParameterName = "Score",
            Value = score
          });

          var result = cmd.ExecuteNonQuery();
          Debug.Log("insert score: " + result);
          debugBoard.text = $"insert score: {result} \n" + debugBoard.text;
        }
      }
    }

    public void GetHighScores(int limit) {
      using (var conn = new SqliteConnection(dbPath)) {
        conn.Open();
        using (var cmd = conn.CreateCommand()) {
          cmd.CommandType = CommandType.Text;
          cmd.CommandText = "SELECT * FROM high_score ORDER BY score DESC LIMIT @Count;";

          cmd.Parameters.Add(new SqliteParameter {
            ParameterName = "Count",
            Value = limit
          });

          Debug.Log("scores (begin)");
          var reader = cmd.ExecuteReader();
          while (reader.Read()) {
            var id = reader.GetInt32(0);
            var highScoreName = reader.GetString(1);
            var score = reader.GetInt32(2);
            var text = string.Format("{0}: {1} [#{2}]", highScoreName, score, id);
            Debug.Log(text);
            debugBoard.text = $"{text}\t" + debugBoard.text;
          }
          Debug.Log("scores (end)");
          debugBoard.text = "\nscores (end) \n" + debugBoard.text;
        }
      }
    }

}

