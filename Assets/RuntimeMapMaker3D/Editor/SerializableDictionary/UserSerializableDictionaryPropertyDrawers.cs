using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[CustomPropertyDrawer(typeof(StringStringDictionary))]
//[CustomPropertyDrawer(typeof(ObjectColorDictionary))]
//[CustomPropertyDrawer(typeof(StringGameObjectDictionary))]
//[CustomPropertyDrawer(typeof(StringSpriteDictionary))]
public class AnySerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer {}

//[CustomPropertyDrawer(typeof(ColorArrayStorage))]
public class AnySerializableDictionaryStoragePropertyDrawer: SerializableDictionaryStoragePropertyDrawer {}
