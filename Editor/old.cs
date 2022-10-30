namespace MegaPint.Editor
{
    public class old
    {
        
                                /*_MassEditNamingReplaceSpaces = EditorGUILayout.Toggle( "Replace Spaces", _MassEditNamingReplaceSpaces );
                                if ( !_MassEditNamingReplaceSpaces ) {
                                    _MassEditSearchToReplace = EditorGUILayout.TextField( "String",  _MassEditSearchToReplace);
                                }
                                
                                _MassEditReplaceString = EditorGUILayout.TextField( "Replace with", _MassEditReplaceString );  
                                if ( !_MassEditReplaceString.Equals( "" ) && ( !_MassEditSearchToReplace.Equals( "" ) || _MassEditNamingReplaceSpaces ) ) action = true;
                                
                                EditorGUILayout.EndVertical(  );
                                EditorGUILayout.Separator();
                            }
                            if ( _MassEditNamingInsert ) {
                                EditorGUILayout.BeginVertical( );

                                _MassEditInsertString = EditorGUILayout.TextField( "String", _MassEditInsertString );
                                _MassEditInsertAt = EditorGUILayout.IntField( "Insert at", _MassEditInsertAt );
                                
                                if (!_MassEditInsertString.Equals( "" ) && _MassEditInsertAt > -1) action = true;
                                
                                EditorGUILayout.EndVertical(  );
                                EditorGUILayout.Separator();
                            }

                            if ( _MassEditNamingCounting ) {
                                EditorGUILayout.BeginVertical( );

                                EditorGUILayout.HelpBox("Work in Progress", MessageType.Info);
                                
                                /*if ( _MassEditObjects != null ) {
                                    _MassEditScrollPos = EditorGUILayout.BeginScrollView( _MassEditScrollPos, GUILayout.Width(width), GUILayout.Height(100) );

                                    for ( var i = 0; i < _MassEditObjects.Count; i++ ) {
                                        _MassEditObjects[i] = EditorGUILayout.ObjectField("Object " + i, _MassEditObjects[i], typeof(Object), true);
                                    }
                                
                                    EditorGUILayout.EndScrollView(  );   
                                }
                                else {
                                    _MassEditObjects = new List<Object>{null};
                                }

                                EditorGUILayout.BeginHorizontal( );
                                
                                if ( GUILayout.Button( "Add Object" ) ) {
                                    _MassEditObjects.Add( null );
                                }
                                
                                if ( GUILayout.Button( "Remove Object" ) ) {
                                    if (_MassEditObjects.Count > 1) _MassEditObjects.RemoveAt( _MassEditObjects.Count - 1 );
                                }
                                
                                EditorGUILayout.EndHorizontal(  );
                                
                                EditorGUILayout.EndVertical(  );
                                EditorGUILayout.Separator();
                            }
                        }

                        EditorGUILayout.Separator();
                        if ( action && (objs.Length > 0 || _MassEditNamingCounting) ) {
                            if (GUILayout.Button("Apply Changes")) {
                                if ( _MassEditNamingRemove ) {
                                    foreach ( var o in objs ) {
                                        var objName = o.name;
                                        objName = objName.Replace( _MassEditNamingRemoveSpaces ? " " : _MassEditSearchToRemove, "" );
                                        
                                        var path = AssetDatabase.GetAssetPath( o );
                                        AssetDatabase.RenameAsset( path, objName );
                                    }
                                }

                                if ( _MassEditNamingReplace ) {
                                    foreach ( var o in objs ) {
                                        var objName = o.name;
                                        objName = objName.Replace( _MassEditNamingReplaceSpaces ? " " : _MassEditSearchToReplace, _MassEditReplaceString );

                                        var path = AssetDatabase.GetAssetPath( o );
                                        AssetDatabase.RenameAsset( path, objName );
                                    }
                                }

                                if ( _MassEditNamingInsert ) {
                                    foreach ( var o in objs ) {
                                        var objName = o.name;
                                        objName = objName.Insert( _MassEditInsertAt, _MassEditInsertString );
                                        
                                        var path = AssetDatabase.GetAssetPath( o );
                                        AssetDatabase.RenameAsset( path, objName );
                                    }
                                }
                                
                                AssetDatabase.Refresh();
                            }
                        }
                        else EditorGUILayout.HelpBox( "No Options selected", MessageType.Error );

                        EditorGUILayout.Separator();
                        DrawUILine( Color.grey, 1, 0 );

                        #endregion
                    }
                    
                    if ( GUILayout.Button( "Prefab Creator" ) ) {
                        _ManagementMassEdit = false;
                        _ManagementPrefabCreator = true;
                    }
                    if ( _ManagementPrefabCreator ) {
                        #region PrefabCreator

                        EditorGUILayout.LabelField("Source Object", EditorStyles.boldLabel);
                        _prefabCreatorSource = (GameObject) EditorGUILayout.ObjectField(_prefabCreatorSource, typeof(GameObject), true);
                    
                        if (_prefabCreatorSource == null) EditorGUILayout.HelpBox( "No Source Object selected", MessageType.Warning );
                        _prefabCreatorCreateInMeshChild = EditorGUILayout.Toggle( "Create inside Mesh-Child", _prefabCreatorCreateInMeshChild );
                    
                        EditorGUILayout.Separator(  );
                    
                        EditorGUILayout.LabelField("Collider", EditorStyles.boldLabel);
                        _prefabCreatorSelectedCollider = EditorGUILayout.Popup("Collidertype", _prefabCreatorSelectedCollider, _prefabCreatorColliderOptions);

                        switch (_prefabCreatorSelectedCollider) {
                            case 0: _prefabCreatorColliderOnMesh = false; break;
                            case 1 or 2 or 3:
                                _prefabCreatorNumberOfColliders = EditorGUILayout.IntSlider("Quantity", _prefabCreatorNumberOfColliders, 1, 8);
                                _prefabCreatorColliderOnMesh = EditorGUILayout.Toggle("Collider on Mesh", _prefabCreatorColliderOnMesh);
                                break;
                            case 4:
                                _prefabCreatorMeshSource = (Mesh) EditorGUILayout.ObjectField( _prefabCreatorMeshSource, typeof( Mesh ), true );
                                if (_prefabCreatorMeshSource == null) EditorGUILayout.HelpBox( "No Mesh Source selected", MessageType.Warning );

                                _prefabCreatorConvex = EditorGUILayout.Toggle( "Convex", _prefabCreatorConvex );
                                _prefabCreatorColliderOnMesh = EditorGUILayout.Toggle("Collider on Mesh", _prefabCreatorColliderOnMesh);
                                break;
                        }

                        EditorGUILayout.Separator();
                        EditorGUILayout.LabelField( "Current Path: " + _prefabCreatorFolderPath );
                        if ( GUILayout.Button( "Select Path" ) ) {
                            _prefabCreatorFolderPath = EditorUtility.OpenFolderPanel("Select Folder to safe to", "", "");
                        }
                        
                        Debug.Log(_prefabCreatorFolderPath);
                        if (_prefabCreatorFolderPath.Equals( "" )) EditorGUILayout.HelpBox( "No Path found", MessageType.Warning );
            
                        _prefabCreatorFileName = EditorGUILayout.TextField("Prefab Name", _prefabCreatorFileName );
            
                        if (_prefabCreatorFileName.Equals( "" )) EditorGUILayout.HelpBox( "No FileName selected", MessageType.Warning );

                        EditorGUILayout.Separator(  );

                        _prefabCreatorOverwriteExisting = EditorGUILayout.Toggle( "Overwrite existing Prefabs", _prefabCreatorOverwriteExisting );
                        
                        if ( File.Exists( _prefabCreatorFolderPath + "/" +_prefabCreatorFileName + ".prefab" ) && !_prefabCreatorOverwriteExisting ) {
                            EditorGUILayout.HelpBox( "File already exists", MessageType.Error );
                        }
                        else if (GUILayout.Button("Create Prefab")) {
                            CreatePrefab();
                        }

                        EditorGUILayout.Separator();
                        DrawUILine( Color.grey, 1, 0 );
                        
                        #endregion
                    }

                    EditorGUILayout.EndVertical( );

                    #endregion

                    break;
                case 1:

                    #region Rendering

                    EditorGUILayout.BeginVertical( );

                    EditorGUILayout.Separator(  );
                    EditorGUILayout.LabelField( "Rendering", h2,GUILayout.ExpandWidth( true ), GUILayout.Height( 20 ) );

                    if ( GUILayout.Button( "EVM Render Tool" ) ) {
                        _RenderingRenderTool = true;
                    }
                    if ( _RenderingRenderTool ) {
                        #region RenderTool

                        EditorGUILayout.LabelField( "Rendering Camera", EditorStyles.boldLabel);
                        _renderToolCam = (Camera) EditorGUILayout.ObjectField(_renderToolCam, typeof(Camera), true);
            
                        if (_renderToolCam == null) EditorGUILayout.HelpBox( "No Rendering Camera selected", MessageType.Warning );
            
                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField( "Render Settings", EditorStyles.boldLabel);

                        _renderToolResolution = EditorGUILayout.Popup("Resolution", _renderToolResolution, _renderToolResolutionsEnum);
                        _renderToolStrengthNormal = EditorGUILayout.Slider("Normal Strength", _renderToolStrengthNormal, 0,1);
                        if (_renderToolStrengthNormal == 0) EditorGUILayout.HelpBox( "Normal Strength cant be 0", MessageType.Warning );
                        _renderToolStrengthGlow = EditorGUILayout.Slider("Glow Strength", _renderToolStrengthGlow, 0,1);
                        if (_renderToolStrengthGlow == 0) EditorGUILayout.HelpBox( "Glow Strength of 0 will ignore PostProcessing", MessageType.Info );
            
                        EditorGUILayout.Separator();
                        EditorGUILayout.LabelField( "Current Path: " + _renderToolFolderPath );
                        if ( GUILayout.Button( "Select Path" ) ) {
                            _renderToolFolderPath = EditorUtility.OpenFolderPanel("Select Folder to safe to", "", "");
                        }
            
                        if (_renderToolFolderPath.Equals( "" )) EditorGUILayout.HelpBox( "No Path found", MessageType.Warning );
            
                        _renderToolFileName = EditorGUILayout.TextField("File Name", _renderToolFileName );
            
                        if (_renderToolFileName.Equals( "" )) EditorGUILayout.HelpBox( "No FileName selected", MessageType.Warning );

                        EditorGUILayout.Separator(  );
            
                        if ( GUILayout.Button( "Render Image" ) ) {
                            RenderImage(  );
                        }
                        
                        EditorGUILayout.Separator(  );
                        DrawUILine( Color.grey, 1, 0 );

                        #endregion   
                    }

                    EditorGUILayout.EndVertical( );

                    #endregion

                    break;
            }
        }

        private static void DrawUILine(Color color, int thickness, int padding) {
            var r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
            r.height = thickness;
            r.y += padding * .5f;
            EditorGUI.DrawRect(r, color);
        }
    
        #region RenderTool Methods

        private void RenderImage( ) {
        
            if ( _renderToolFolderPath.Equals( "" ) || _renderToolFileName.Equals( "" ) || _renderToolCam == null ) return;
            if (_renderToolStrengthNormal == 0) return;
        
            var normal = Render(GraphicsFormat.R8G8B8A8_SRGB, GraphicsFormat.D32_SFloat_S8_UInt);
            var glow = Render(GraphicsFormat.R32G32B32A32_SFloat, GraphicsFormat.D32_SFloat_S8_UInt);
        
            var normalTexture = ConvertToTexture(normal);
            var glowTexture = ConvertToTexture(glow);
                    
            var bytes = MixImages(normalTexture, glowTexture).EncodeToPNG();
            File.WriteAllBytes(_renderToolFolderPath + "/" + _renderToolFileName + ".png", bytes);
            AssetDatabase.Refresh();
                    
            DestroyImmediate(normal);
            DestroyImmediate(glow);
        }
        
        private RenderTexture Render(GraphicsFormat format, GraphicsFormat depth) {
            var result = new RenderTexture(_renderToolResolutions[_renderToolResolution], _renderToolResolutions[_renderToolResolution],
                format, depth);
            _renderToolCam.targetTexture = result;
            _renderToolCam.Render();
            _renderToolCam.targetTexture = null;
            return result;
        }
                
        private static Texture2D ConvertToTexture(RenderTexture target) {
            var image = new Texture2D(target.width, target.height);
            RenderTexture.active = target;
            image.ReadPixels(new Rect(0, 0, target.width, target.height), 0, 0);
            image.Apply( );
            RenderTexture.active = null;
            return image;
        }
        
        private Texture2D MixImages(Texture2D basic, Texture2D glow) {
            var result = new Texture2D(basic.width, basic.height);
            for ( var i = 0; i < basic.width; i++ ) {
                for ( var j = 0; j < basic.height; j++ ) {
                    var basicColor = basic.GetPixel( i, j );
                    var glowColor = glow.GetPixel( i, j );
                    result.SetPixel(i, j, basicColor * _renderToolStrengthNormal + glowColor * _renderToolStrengthGlow);
                }
            }
            return result;
        }

        #endregion

        #region PrefabCreator Methods

        private void CreatePrefab() {

            GameObject sceneObject;
            GameObject mesh = null;
            
            if ( !_prefabCreatorCreateInMeshChild ) sceneObject = Instantiate( _prefabCreatorSource );
            else {
                sceneObject = new GameObject( _prefabCreatorFileName );

                mesh = Instantiate( _prefabCreatorSource, sceneObject.transform );
                mesh.name = "Mesh";
            }
            
            sceneObject.name = _prefabCreatorFileName;
            sceneObject.transform.localScale = Vector3.one * (_prefabCreatorCreateInMeshChild ? 1 : 0.0001f);
            sceneObject.transform.position = Vector3.zero;
            sceneObject.transform.rotation = Quaternion.Euler(0,0,0);

            if ( _prefabCreatorCreateInMeshChild && mesh is not null ) {
                mesh.transform.localScale = Vector3.one * 0.0001f;
                mesh.transform.localPosition = Vector3.zero;
            }

            GameObject collider = null;

            if ( _prefabCreatorColliderOnMesh ) {
                switch ( _prefabCreatorSelectedCollider ) {
                    case 1 or 2 or 3:
                        for ( var i = 0; i < _prefabCreatorNumberOfColliders; i++ ) {
                            if ( _prefabCreatorCreateInMeshChild && mesh is not null ) {
                                switch ( _prefabCreatorSelectedCollider ) {
                                    case 1: mesh.AddComponent<BoxCollider>( ); break;
                                    case 2: mesh.AddComponent<CapsuleCollider>( ); break;
                                    case 3: mesh.AddComponent<SphereCollider>( ); break;
                                }
                            }
                            else {
                                switch ( _prefabCreatorSelectedCollider ) {
                                    case 1: sceneObject.AddComponent<BoxCollider>( ); break;
                                    case 2: sceneObject.AddComponent<CapsuleCollider>( ); break;
                                    case 3: sceneObject.AddComponent<SphereCollider>( ); break;
                                }
                            }

                        }
                        break;
                    case 4:
                        MeshCollider meshCollider;
                        if ( _prefabCreatorCreateInMeshChild && mesh is not null ) meshCollider = mesh.AddComponent<MeshCollider>( );
                        else meshCollider = sceneObject.AddComponent<MeshCollider>( );
                        
                        meshCollider.sharedMesh = _prefabCreatorMeshSource;
                        meshCollider.convex = _prefabCreatorConvex;
                        break;
                }
            }
            else {
                switch ( _prefabCreatorSelectedCollider ) {
                    case 1 or 2 or 3:
                        collider = new GameObject( "Collider" );
                        collider.transform.parent = sceneObject.transform;

                        for ( var i = 0; i < _prefabCreatorNumberOfColliders; i++ ) {
                            switch ( _prefabCreatorSelectedCollider ) {
                                case 1: collider.AddComponent<BoxCollider>( ); break;
                                case 2: collider.AddComponent<CapsuleCollider>( ); break;
                                case 3: collider.AddComponent<SphereCollider>( ); break;
                            }
                        }
                    
                        break;
                    case 4:
                        collider = new GameObject( "Collider" );
                        collider.transform.parent = sceneObject.transform;
                        collider.transform.localScale = Vector3.one * 0.0001f;
                        if ( mesh is not null ) collider.transform.localRotation = mesh.transform.localRotation;

                        var meshCollider = collider.AddComponent<MeshCollider>( );
                        meshCollider.sharedMesh = _prefabCreatorMeshSource;
                        meshCollider.convex = _prefabCreatorConvex;
                        break;
                }   
            }

            PrefabUtility.SaveAsPrefabAssetAndConnect(sceneObject, _prefabCreatorFolderPath + "/" +_prefabCreatorFileName + ".prefab", InteractionMode.UserAction, out _);
            DestroyImmediate(sceneObject);
            if (mesh is not null) DestroyImmediate(mesh);
            if ( collider is not null ) DestroyImmediate( collider );

            AssetDatabase.Refresh();
        }

        #endregion
    }*/
    }
}