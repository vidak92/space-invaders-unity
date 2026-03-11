using System.Collections.Generic;
using SGSTools.Extensions;
using SGSTools.Util;
using UnityEngine;

namespace SpaceInvaders
{
    public class DestructibleQuad : MonoBehaviour
    {
        public QuadMeshRenderer QuadMeshRenderer;
        public Rigidbody2D Rigidbody;

        private Vector2 _tiling;
        private Vector4 _st;
        private MaterialPropertyBlock _block;
        
        private Vector2Int _gridSize = new Vector2Int(
            Constants.DESTRUCTIBLE_QUAD_GRID_COL_COUNT, 
            Constants.DESTRUCTIBLE_QUAD_GRID_ROW_COUNT);

        private List<float> _flags = new();
        private List<Collider2D> _contacts = new();

        private GameController GameController => ServiceLocator.Get<GameController>();

        public void Init()
        {
            _tiling = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));
            _block = new MaterialPropertyBlock();
            
            _flags.Clear();
            for (int i = 0; i < _gridSize.x * _gridSize.y; i++)
            {
                _flags.Add(1f);
            }
        }

        private void Update()
        {
            QuadMeshRenderer.Material.SetFloat(Constants.DESTRUCTIBLE_QUAD_SHADER_PROPERTY_COLS, _gridSize.x);
            QuadMeshRenderer.Material.SetFloat(Constants.DESTRUCTIBLE_QUAD_SHADER_PROPERTY_ROWS, _gridSize.y);
            
            var cellSize = new Vector2(transform.localScale.x / _gridSize.x, transform.localScale.y / _gridSize.y);
            var gridStartPosition = transform.position - new Vector3(_gridSize.x / 2f * cellSize.x, _gridSize.y / 2f * cellSize.y, 0f);
            for (int i = 0; i <= _gridSize.x; i++)
            {
                var position1 = gridStartPosition + new Vector3(cellSize.x * i, 0f, 0f);
                var position2 = gridStartPosition + new Vector3(cellSize.x * i, cellSize.y * _gridSize.y, 0f);
                DebugDraw.DrawLine(position1, position2, Color.magenta);
            }
            for (int i = 0; i <= _gridSize.y; i++)
            {
                var position1 = gridStartPosition + new Vector3(0f, cellSize.y * i, 0f);
                var position2 = gridStartPosition + new Vector3(cellSize.x * _gridSize.x, cellSize.y * i, 0f);
                DebugDraw.DrawLine(position1, position2, Color.magenta);
            }
            
            Vector2 halfGridSize = new Vector2(_gridSize.x / 2f, _gridSize.y / 2f);
            Rigidbody.GetContacts(_contacts);
            foreach (var contact in _contacts)
            {
                var projectile = contact.GetComponent<Projectile>();
                if (projectile != null)
                {
                    var hitPosition = projectile.Rigidbody.position + Vector2.up * projectile.TipOffsetY;
                    // clamp to bounds
                    hitPosition.x = Mathf.Clamp(hitPosition.x, transform.position.x - halfGridSize.x, transform.position.x + halfGridSize.x);
                    hitPosition.y = Mathf.Clamp(hitPosition.y, transform.position.y - halfGridSize.y, transform.position.y + halfGridSize.y);
                    
                    var localHitPosition = transform.InverseTransformPoint(hitPosition);
                    localHitPosition += new Vector3(0.5f, 0.5f, 0f); // map to [0-1]
                    
                    var hitIndex = new Vector2Int(
                        Mathf.FloorToInt(localHitPosition.x * _gridSize.x),
                        Mathf.FloorToInt(localHitPosition.y * _gridSize.y));

                    // clamp index if out of bounds
                    hitIndex.x = Mathf.Clamp(hitIndex.x, 0, _gridSize.x - 1);
                    hitIndex.y = Mathf.Clamp(hitIndex.y, 0, _gridSize.y - 1);

                    // check if any grid cells were skipped over
                    if (projectile.Direction == ProjectileDirection.Up)
                    {
                        var newIndexY = 0;
                        while (newIndexY < hitIndex.y)
                        {
                            var newFlagIndex = GetFlagIndex(hitIndex.x, newIndexY);
                            if (_flags[newFlagIndex] > 0f)
                            {
                                hitIndex.y = newIndexY;
                                break;
                            }
                            newIndexY++;
                        }
                    }
                    else
                    {
                        var newIndexY = _gridSize.y - 1;
                        while (newIndexY > hitIndex.y)
                        {
                            var newFlagIndex = GetFlagIndex(hitIndex.x, newIndexY);
                            if (_flags[newFlagIndex] > 0f)
                            {
                                hitIndex.y = newIndexY;
                                break;
                            }
                            newIndexY--;
                        }
                    }
                    
                    var flagIndex = GetFlagIndex(hitIndex.x, hitIndex.y);
                    if (_flags[flagIndex] > 0f)
                    {
                        // Debug.Log($"{gameObject.name} hit at grid index: {hitIndex}");
                        _flags[flagIndex] = 0f;
                        GameController.OnEnvironmentHit(projectile, hitPosition);
                        break;
                    }
                }
            }

            QuadMeshRenderer.MeshRenderer.GetPropertyBlock(_block);
            _block.SetFloatArray(Constants.DESTRUCTIBLE_QUAD_SHADER_PROPERTY_FLAGS, _flags);
            _st = new Vector4(0.4f, 0.2f, _tiling.x, _tiling.y);
            _block.SetVector(Constants.DESTRUCTIBLE_QUAD_SHADER_PROPERTY_MAIN_TEX_ST, _st);
            QuadMeshRenderer.MeshRenderer.SetPropertyBlock(_block);
        }
        
        public void ResetFlags()
        {
            for (int i = 0; i < _flags.Count; i++)
            {
                _flags[i] = 1f;
            }
        }

        private int GetFlagIndex(int x, int y)
        {
            int index = x + y * _gridSize.x;
            if (!_flags.ContainsIndex(index))
            {
                Debug.LogError($"{gameObject.name}: cannot get flag index for (x:{x}, y: {y}), returning -1");
                return -1;
            }
            return index;
        }
    }
}