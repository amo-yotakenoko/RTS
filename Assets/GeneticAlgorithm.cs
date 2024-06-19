using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class GeneticAlgorithm : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject characterPrefab;
    public List<Vector3> basePositions;
    public List<AIcommand> AIcommands;
    public AIcommand.Parameter strongParameter;
    void Start()
    {
        reset();
    }
    public float matchTime;
    public float timescale;

    // Update is called once per frame
    void Update()
    {





        Time.timeScale = timescale;
        matchTime += Time.deltaTime;
        int remainingTeamCOunt = 0;
        int remainingTeam = 0;
        foreach (var command in AIcommands)
        {
            int t = command.team;
            List<Character> teamCharacters = GameObject.FindGameObjectsWithTag("entity").Select(x => x.GetComponent<Character>()).Where(x => x != null && x.team == t).ToList();
            if (teamCharacters.Count > 0)
            {
                remainingTeamCOunt++;
                remainingTeam = t;
            }
        }

        // if (remainingTeamCOunt == 1)
        // {
        //     // print(AIcommands.Where(x => x.team == remainingTeam).FirstOrDefault().parameter);
        //     strongParameter = AIcommands.Where(x => x.team == remainingTeam).FirstOrDefault().parameter;

        //     FileWriter fileWriter = new FileWriter();
        //     print(Application.persistentDataPath);
        //     string filePath = Path.Combine(Application.persistentDataPath, "parameter.txt");
        //     fileWriter.AppendToFile(filePath, strongParameter.ToString(););
        //     reset();
        // }
        if (matchTime > 100 || remainingTeamCOunt == 1)
        {

            // strongParameter = AIcommands.Where(x => x.team == remainingTeam).FirstOrDefault().parameter;
            string filePath = Path.Combine(Application.persistentDataPath, "parameter.txt");
            print(Application.persistentDataPath);
            using (StreamWriter fileWriter = new StreamWriter(filePath, true))
            {
                fileWriter.WriteLine(strongParameter.ToString());
            }

            reset();

        }
        // print(remainingTeamCOunt);
    }


    int GetTeamWithMostScore()
    {
        // チームごとのキャラクター数をカウントする辞書
        // var teamCounts = GameObject.FindGameObjectsWithTag("entity")
        //     .Select(x => x.GetComponent<Character>())
        //     .Where(x => x != null)
        //     .GroupBy(x => x.team)
        //     .Select(g => new { Team = g.Key, Count = g.Count() })
        //     .ToDictionary(x => x.Team, x => x.Count);

        // // すべてのAIcommandsの中から最大のキャラクター数を持つチームを取得する
        // var maxTeam = AIcommands
        //     .Select(command => command.team)
        //     .GroupBy(team => team)
        //     .Select(g => new { Team = g.Key, Count = teamCounts.ContainsKey(g.Key) ? teamCounts[g.Key] : 0 })
        //     .OrderByDescending(tc => tc.Count)
        //     .FirstOrDefault();
        var mostScoreTeam = AIcommands.OrderByDescending(x => x.sumAddDamage + x.countCharacters()).FirstOrDefault();


        return mostScoreTeam != null ? mostScoreTeam.team : -1; // キャラクターがいない場合、-1 を返す
    }

    void reset()
    {
        matchTime = 0;
        foreach (var c in GameObject.FindGameObjectsWithTag("entity"))
        {
            Destroy(c);
        }
        foreach (var command in AIcommands)
        {
            Destroy(command);
        }
        AIcommands = new List<AIcommand>();

        int teamid = 1;
        foreach (var basePos in basePositions)
        {
            for (int i = 0; i < 25; i++)
            {
                var character = Instantiate(characterPrefab, basePos, Quaternion.identity);
                character.GetComponent<Entity>().team = teamid;
                character.GetComponent<UnityEngine.AI.NavMeshAgent>().destination = new Vector3(0, 0, 0);
                // character.transform.position += new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
                character.transform.position = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f)) * 20f;
            }
            AIcommand command = gameObject.AddComponent<AIcommand>();
            command.team = teamid;
            command.parameter = new AIcommand.Parameter();
            command.parameter = strongParameter;
            command.parameter.RandomizeParameters();
            AIcommands.Add(command);
            //    LineRenderer line = AIcommands.AddComponent<LineRenderer>();
            teamid++;

        }
    }
}
