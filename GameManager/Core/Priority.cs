using System.Collections;
using UnityEngine;

/*
 * The way priorities work is that the list is ordered
 * in descending order of priority. They each have a condition 
 * that, if not fulfilled, is skipped.
 * -MKL
 */

public enum RelevanceEnum
{
    TOSELF,
    TOFRIENDLIES,
    TOENEMIES,
}

public enum CharacterStatusEnum
{
    ANY,
    ISDEAD,
    ISLOWERTHAN50,
    ISLOWERTHAN25,
    ISPOISONED,
}

public class Condition
{
    public RelevanceEnum relevance; //as opposed to enemy
    public CharacterStatusEnum status;
}

public class Priority
{
    public Condition condition;
    public BattleChoice choice;    
}
