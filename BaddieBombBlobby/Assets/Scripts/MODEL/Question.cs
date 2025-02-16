using System;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// MODEL CLASSES
public class Question
{
    public int Id { get; set; }
    public string Text { get; set; }
}
