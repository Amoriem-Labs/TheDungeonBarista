// Assets/Editor/CsvDialogImporterWindow.cs
#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace TDB.Editor
{
    public class CsvDialogImporterWindow : EditorWindow
    {
        private DefaultAsset? _dialogOutputFolder;
        private DefaultAsset? _actorSearchFolder;
        private TextAsset? _csvAsset;

        private bool _hasHeader = false;
        private bool _overwriteExisting = true;

        [MenuItem("Tools/Dialog/CSV Importer (SimpleDialogGroup)")]
        public static void Open()
        {
            var w = GetWindow<CsvDialogImporterWindow>();
            w.titleContent = new GUIContent("CSV Dialog Importer");
            w.minSize = new Vector2(520, 260);
            w.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("CSV → SimpleDialogGroup Importer", EditorStyles.boldLabel);
            EditorGUILayout.Space(6);

            _csvAsset = (TextAsset?)EditorGUILayout.ObjectField(
                new GUIContent("CSV File (TextAsset)", "Drag in a .csv TextAsset (recommended)"),
                _csvAsset, typeof(TextAsset), false);

            // Optional: allow picking a raw CSV file path instead of TextAsset
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Or CSV Path", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
            if (GUILayout.Button("Select .csv...", GUILayout.Height(18)))
            {
                var path = EditorUtility.OpenFilePanel("Select CSV", Application.dataPath, "csv");
                if (!string.IsNullOrEmpty(path))
                {
                    // Create a temp TextAsset-like read at import time; we’ll store path in EditorPrefs
                    EditorPrefs.SetString("CsvDialogImporterWindow_LastCsvPath", path);
                    // Keep _csvAsset as-is; import uses either asset or saved path.
                }
            }
            var lastPath = EditorPrefs.GetString("CsvDialogImporterWindow_LastCsvPath", "");
            EditorGUILayout.SelectableLabel(string.IsNullOrEmpty(lastPath) ? "(none selected)" : lastPath,
                GUILayout.Height(18));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(8);

            _dialogOutputFolder = (DefaultAsset?)EditorGUILayout.ObjectField(
                new GUIContent("Dialog Output Folder", "Where SimpleDialogGroup assets will be created"),
                _dialogOutputFolder, typeof(DefaultAsset), false);

            _actorSearchFolder = (DefaultAsset?)EditorGUILayout.ObjectField(
                new GUIContent("Actor Folder", "Where ActorDefinition assets named 'DA-[actorID]' are searched/created"),
                _actorSearchFolder, typeof(DefaultAsset), false);

            EditorGUILayout.Space(6);

            _hasHeader = EditorGUILayout.ToggleLeft("CSV has a header row", _hasHeader);
            _overwriteExisting = EditorGUILayout.ToggleLeft("Overwrite existing dialog assets", _overwriteExisting);

            EditorGUILayout.Space(10);

            using (new EditorGUI.DisabledScope(!CanImport()))
            {
                if (GUILayout.Button("Import", GUILayout.Height(32)))
                {
                    try
                    {
                        Import();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                        EditorUtility.DisplayDialog("Import Failed", ex.Message, "OK");
                    }
                }
            }

            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox(
                "CSV columns:\n" +
                "  1) DialogID (non-empty starts a new group; empty continues previous)\n" +
                "  2) ActorID (bind ActorDefinition named 'DA-[ActorID]' from Actor Folder)\n" +
                "  3) Text\n\n" +
                "All rows until the next non-empty DialogID go into the same SimpleDialogGroup asset.",
                MessageType.Info);
        }

        private bool CanImport()
        {
            if (_dialogOutputFolder == null || _actorSearchFolder == null) return false;
            if (_csvAsset != null) return true;

            var lastPath = EditorPrefs.GetString("CsvDialogImporterWindow_LastCsvPath", "");
            return !string.IsNullOrEmpty(lastPath) && File.Exists(lastPath);
        }

        private void Import()
        {
            var dialogFolderPath = AssetDatabase.GetAssetPath(_dialogOutputFolder!);
            var actorFolderPath = AssetDatabase.GetAssetPath(_actorSearchFolder!);

            if (!AssetDatabase.IsValidFolder(dialogFolderPath))
                throw new InvalidOperationException("Dialog Output Folder is not a valid project folder.");
            if (!AssetDatabase.IsValidFolder(actorFolderPath))
                throw new InvalidOperationException("Actor Folder is not a valid project folder.");

            string csvText;
            if (_csvAsset != null)
            {
                csvText = _csvAsset.text;
            }
            else
            {
                var path = EditorPrefs.GetString("CsvDialogImporterWindow_LastCsvPath", "");
                csvText = File.ReadAllText(path, Encoding.UTF8);
            }

            var rows = ReadCsv(csvText);
            if (rows.Count == 0)
            {
                EditorUtility.DisplayDialog("Nothing to Import", "CSV appears empty.", "OK");
                return;
            }

            int startRow = _hasHeader ? 1 : 0;

            // dialogId -> list of (actorId, text)
            var grouped = new Dictionary<string, List<(string actorId, string text)>>();
            string? currentDialogId = null;

            for (int i = startRow; i < rows.Count; i++)
            {
                var cols = rows[i];
                if (cols.Count == 0) continue;

                string c0 = cols.Count > 0 ? cols[0].Trim() : "";
                string c1 = cols.Count > 1 ? cols[1].Trim() : "";
                string c2 = cols.Count > 2 ? cols[2] : ""; // keep inner spaces in text

                // Skip completely empty lines
                if (string.IsNullOrEmpty(c0) && string.IsNullOrEmpty(c1) && string.IsNullOrEmpty(c2))
                    continue;

                if (!string.IsNullOrEmpty(c0))
                {
                    currentDialogId = c0;
                    if (!grouped.ContainsKey(currentDialogId))
                        grouped[currentDialogId] = new List<(string actorId, string text)>();
                }

                if (string.IsNullOrEmpty(currentDialogId))
                    throw new InvalidOperationException($"Row {i + 1}: encountered a line before any DialogID was set.");

                // ActorID and Text can be empty, but usually you want both.
                grouped[currentDialogId].Add((c1, c2));
            }

            // Pre-resolve (and possibly create) all actors; abort if user refuses any creation.
            var actorCache = new Dictionary<string, UnityEngine.Object?>(); // ActorID -> ActorDefinition
            foreach (var kv in grouped)
            {
                foreach (var (actorId, _) in kv.Value)
                {
                    if (string.IsNullOrEmpty(actorId)) continue; // allow empty actor if you want narrator/etc.
                    if (actorCache.ContainsKey(actorId)) continue;

                    var actor = FindActorDefinitionById(actorFolderPath, actorId);
                    if (actor == null)
                    {
                        bool create = EditorUtility.DisplayDialog(
                            "Missing ActorDefinition",
                            $"Could not find ActorDefinition named 'DA-{actorId}' in folder:\n{actorFolderPath}\n\nCreate it now?\n\n(Choosing 'No' aborts the entire import.)",
                            "Create",
                            "Abort Import");

                        if (!create)
                        {
                            EditorUtility.DisplayDialog("Import Aborted", "User chose not to create missing actor assets.", "OK");
                            return;
                        }

                        actor = CreateActorDefinition(actorFolderPath, actorId);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }

                    actorCache[actorId] = actor;
                }
            }

            int created = 0, updated = 0;

            foreach (var (dialogId, lines) in grouped)
            {
                string assetName = $"DG-{dialogId}";
                string assetPath = AssetDatabase.GenerateUniqueAssetPath($"{dialogFolderPath}/{assetName}.asset");

                // If overwrite is enabled, prefer exact path and replace content if exists.
                string preferredPath = $"{dialogFolderPath}/{assetName}.asset";
                var existing = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(preferredPath);

                SimpleDialogGroup group;
                string finalPath;

                if (existing != null && !_overwriteExisting)
                {
                    // If not overwriting, create a unique one
                    finalPath = assetPath;
                    group = ScriptableObject.CreateInstance<SimpleDialogGroup>();
                    AssetDatabase.CreateAsset(group, finalPath);
                    created++;
                }
                else if (existing != null)
                {
                    finalPath = preferredPath;
                    group = AssetDatabase.LoadAssetAtPath<SimpleDialogGroup>(finalPath);
                    if (group == null)
                        throw new InvalidOperationException($"Asset at {finalPath} exists but is not a SimpleDialogGroup.");
                    updated++;
                }
                else
                {
                    finalPath = preferredPath;
                    group = ScriptableObject.CreateInstance<SimpleDialogGroup>();
                    AssetDatabase.CreateAsset(group, finalPath);
                    created++;
                }

                // Fill _statements using SerializedObject so we can set private fields too.
                var so = new SerializedObject(group);
                var statementsProp = so.FindProperty("_statements");
                if (statementsProp == null || !statementsProp.isArray)
                    throw new InvalidOperationException("Could not find serialized list field '_statements' on SimpleDialogGroup.");

                statementsProp.arraySize = lines.Count;

                for (int i = 0; i < lines.Count; i++)
                {
                    var (actorId, text) = lines[i];
                    var element = statementsProp.GetArrayElementAtIndex(i);

                    // ActorStatement has private field _actor, and public field Statement.
                    var actorProp = element.FindPropertyRelative("_actor");
                    if (actorProp == null)
                        throw new InvalidOperationException("Could not find '_actor' field on ActorStatement (serialization mismatch).");

                    if (!string.IsNullOrEmpty(actorId))
                        actorProp.objectReferenceValue = actorCache[actorId];
                    else
                        actorProp.objectReferenceValue = null;

                    var statementProp = element.FindPropertyRelative("Statement");
                    if (statementProp == null)
                        throw new InvalidOperationException("Could not find 'Statement' field on ActorStatement (serialization mismatch).");

                    if (!TrySetStatementText(statementProp, text))
                    {
                        Debug.LogWarning(
                            $"[{nameof(CsvDialogImporterWindow)}] Could not set Statement text for dialog '{dialogId}', line {i + 1}. " +
                            $"Statement type is '{statementProp.type}'. Consider adding a known string field name like 'Text' inside Statement.");
                    }
                }

                so.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(group);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog(
                "Import Complete",
                $"Dialogs created: {created}\nDialogs updated: {updated}\n\nOutput: {_dialogOutputFolder!.name}",
                "OK");
        }

        // ----------------------------
        // ActorDefinition lookup/create
        // ----------------------------

        private static UnityEngine.Object? FindActorDefinitionById(string actorFolderPath, string actorId)
        {
            string targetName = $"DA-{actorId}";
            // Search within folder for matching name
            var guids = AssetDatabase.FindAssets($"{targetName} t:ScriptableObject", new[] { actorFolderPath });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                if (obj != null && obj.name == targetName && obj.GetType().Name == "ActorDefinition")
                    return obj;
            }

            // If FindAssets string query didn’t match well, do a more direct scan of ActorDefinition
            guids = AssetDatabase.FindAssets("t:ActorDefinition", new[] { actorFolderPath });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                if (obj != null && obj.name == targetName)
                    return obj;
            }

            return null;
        }

        private static UnityEngine.Object CreateActorDefinition(string actorFolderPath, string actorId)
        {
            // We don’t have compile-time access to ActorDefinition type here (it exists in your project),
            // but Unity can still create it via reflection by name if it’s in loaded assemblies.
            var actorType = FindTypeByName("ActorDefinition");
            if (actorType == null)
                throw new InvalidOperationException("Could not find type 'ActorDefinition' in loaded assemblies.");

            var actor = ScriptableObject.CreateInstance(actorType);
            actor.name = $"DA-{actorId}";

            string path = $"{actorFolderPath}/{actor.name}.asset";
            path = AssetDatabase.GenerateUniqueAssetPath(path);
            AssetDatabase.CreateAsset(actor, path);

            return actor;
        }

        private static Type? FindTypeByName(string typeName)
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                // Skip dynamic/reflection-only assemblies if any
                if (asm.IsDynamic) continue;
                var t = asm.GetType(typeName);
                if (t != null) return t;

                // Also try full scan by short name
                try
                {
                    foreach (var candidate in asm.GetTypes())
                    {
                        if (candidate.Name == typeName)
                            return candidate;
                    }
                }
                catch { /* ignore assemblies that throw */ }
            }
            return null;
        }

        // ----------------------------
        // Statement text assignment
        // ----------------------------

        private static bool TrySetStatementText(SerializedProperty statementProp, string text)
        {
            // If Statement itself is a string field:
            if (statementProp.propertyType == SerializedPropertyType.String)
            {
                statementProp.stringValue = text;
                return true;
            }

            // If Statement is a managed reference, ensure it exists
            if (statementProp.propertyType == SerializedPropertyType.ManagedReference && statementProp.managedReferenceValue == null)
            {
                // Try to instantiate its declared type if possible
                var declaredType = GetManagedReferenceFieldType(statementProp);
                if (declaredType != null && declaredType.GetConstructor(Type.EmptyTypes) != null)
                    statementProp.managedReferenceValue = Activator.CreateInstance(declaredType);
            }

            // Common inner string field names to try:
            string[] candidates =
            {
                "Text", "text",
                "Content", "content",
                "Value", "value",
                "Line", "line",
                "Message", "message"
            };

            foreach (var name in candidates)
            {
                var p = statementProp.FindPropertyRelative(name);
                if (p != null && p.propertyType == SerializedPropertyType.String)
                {
                    p.stringValue = text;
                    return true;
                }
            }

            // Last resort: find the first string field inside Statement (shallow search)
            // Note: SerializedProperty iteration is finicky; keep it simple.
            var copy = statementProp.Copy();
            var end = copy.GetEndProperty();
            bool enterChildren = true;
            while (copy.NextVisible(enterChildren) && !SerializedProperty.EqualContents(copy, end))
            {
                enterChildren = false;

                // Only look at direct children: depth == statementProp.depth + 1
                if (copy.depth != statementProp.depth + 1) continue;

                if (copy.propertyType == SerializedPropertyType.String)
                {
                    copy.stringValue = text;
                    return true;
                }
            }

            return false;
        }

        private static Type? GetManagedReferenceFieldType(SerializedProperty prop)
        {
            // Unity doesn’t expose declared type directly; we can’t reliably recover it in all cases.
            // Return null and let the user ensure Statement has a simple string field like Text.
            return null;
        }

        // ----------------------------
        // CSV parsing (supports quotes)
        // ----------------------------

        private static List<List<string>> ReadCsv(string csvText)
        {
            var rows = new List<List<string>>();

            using var sr = new StringReader(csvText);
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                // Skip BOM/zero-width
                if (rows.Count == 0) line = line.TrimStart('\uFEFF');

                // Ignore pure whitespace lines
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                rows.Add(ParseCsvLine(line));
            }

            return rows;
        }

        private static List<string> ParseCsvLine(string line)
        {
            var result = new List<string>();
            var sb = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (inQuotes)
                {
                    if (c == '"')
                    {
                        // Escaped quote?
                        if (i + 1 < line.Length && line[i + 1] == '"')
                        {
                            sb.Append('"');
                            i++;
                        }
                        else
                        {
                            inQuotes = false;
                        }
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                else
                {
                    if (c == ',')
                    {
                        result.Add(sb.ToString());
                        sb.Clear();
                    }
                    else if (c == '"')
                    {
                        inQuotes = true;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }

            result.Add(sb.ToString());
            return result;
        }
    }
}
