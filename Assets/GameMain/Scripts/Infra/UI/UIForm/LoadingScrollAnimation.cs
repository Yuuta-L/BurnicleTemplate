using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Template
{
    public class LoadingScrollAnimation : MonoBehaviour
    {
        public RectTransform window;
        public RectTransform frontRoot;
        public RectTransform backRoot;
        
        public Sprite[] floorSprites;
        public Sprite[] wallSprites;
        public Sprite[] propSprites;
        
        // 滚动速度（像素/秒）
        public float scrollSpeed = 200f;
        //预生成距离
        public float preSpawnDistance = 50;
        // 道具生成位置随机偏移范围（相对于固定间隔位置）
        public float propPositionRandomRange = 30f;
        // 固定间隔位置（用于道具生成）
        public float propFixedInterval = 400f;
        // 地面高度（Y坐标）
        public float groundHeight = 0f;
        
        // 图片池
        private readonly Queue<Image> _imagePool = new Queue<Image>();
        // 当前活跃的滚动图片列表
        private readonly List<Image> _activeImages = new List<Image>();
        
        // 滚动状态
        private bool isPlaying = false;
        private int floorIndex = 0;
        private int wallIndex = 0;
        // 窗口长度
        private float windowLength;
        
        // 已生成的总长度
        private float _totalMoveDistance = 0f;//已行进的逻辑距离
        private float _spawnedFloorLength = 0f;
        private float _spawnedWallLength = 0f;
        private int _spawnedPropCount = 0;
        
        public void PlayAnimation()
        {
            // 停止当前动画，重置状态
            StopAnimation();
            
            // 初始化边界
            windowLength = window.rect.size.x;
            frontRoot.anchoredPosition = backRoot.anchoredPosition = new Vector2(window.rect.xMin, 0);
            isPlaying = true;
            
            
            // 重置状态
            floorIndex = 0;
            wallIndex = 0;
            _totalMoveDistance = 0f;
            _spawnedFloorLength = 0f;
            _spawnedWallLength = 0f;
            _spawnedPropCount = 0;
            
            SpawnImage();
        }
        
        public void StopAnimation()
        {
            isPlaying = false;
            ReleaseAllImages();
        }
        
        private void Update()
        {
            if (!isPlaying) return;
            
            SpawnImage();
            Movement();
            // 回收超出左侧窗口的图片
            RecycleOutOfBoundsImages();
        }

        private void SpawnImage()
        {
            while (_spawnedFloorLength - _totalMoveDistance <= windowLength + preSpawnDistance)
            {
                SpawnNextFloor();
            }
            
            while (_spawnedWallLength - _totalMoveDistance <= windowLength + preSpawnDistance)
            {
                SpawnNextWall();
            }
            
            while (_spawnedPropCount * propFixedInterval - _totalMoveDistance <= windowLength + preSpawnDistance + propPositionRandomRange)
            {
                SpawnNextProp();
            }
        }
        
        private void SpawnNextFloor()
        {
            var sprite = floorSprites[floorIndex % floorSprites.Length];
            var img = GetImageFromPool();
            img.sprite = sprite;
            img.SetNativeSize();


            var spawnPosX = _spawnedFloorLength - _totalMoveDistance;
            var rect = img.rectTransform;
            rect.SetParent(backRoot, false);
            var width = sprite.rect.width;
            var height = sprite.rect.height;
            rect.anchoredPosition = new Vector2(spawnPosX + width * 0.5f, groundHeight - height * 0.5f);
            
            _activeImages.Add(img);
            _spawnedFloorLength += width;
            floorIndex++;
        }
        
        private void SpawnNextWall()
        {
            var sprite = wallSprites[wallIndex % wallSprites.Length];
            var img = GetImageFromPool();
            img.sprite = sprite;
            img.SetNativeSize();
            
            var spawnPosX = _spawnedWallLength - _totalMoveDistance;
            var rect = img.rectTransform;
            rect.SetParent(backRoot, false);
            var width = sprite.rect.width;
            //var height = sprite.rect.height;
            rect.anchoredPosition = new Vector2(spawnPosX + width * 0.5f, 0);
            _activeImages.Add(img);
            _spawnedWallLength += width;
            wallIndex++;
        }
        
        private List<int> _availableIndices = new List<int>(); // Tracks which sprite indices are available
        private int _lastSpriteIndex = -1; // Tracks the last used sprite index

        private void SpawnNextProp()
        {
            if (propSprites == null || propSprites.Length == 0) return;
            
            // Initialize or refill the available indices pool
            if (_availableIndices.Count == 0)
            {
                // Reset available indices with all sprite indices
                _availableIndices.Clear();
                for (int i = 0; i < propSprites.Length; i++)
                {
                    _availableIndices.Add(i);
                }
                
                // Remove the last used sprite from the pool if it exists to prevent consecutive duplicates
                if (_lastSpriteIndex >= 0 && _availableIndices.Contains(_lastSpriteIndex))
                {
                    _availableIndices.Remove(_lastSpriteIndex);
                }
            }
            
            // Randomly select an index from available indices
            var randomIndexInPool = Random.Range(0, _availableIndices.Count);
            var selectedSpriteIndex = _availableIndices[randomIndexInPool];
            _availableIndices.RemoveAt(randomIndexInPool);
            
            // Update last used sprite
            _lastSpriteIndex = selectedSpriteIndex;
            
            // Get the actual sprite
            var sprite = propSprites[selectedSpriteIndex];
            
            // Calculate spawn position: based on current rightmost position + fixed interval
            var spawnPosX = _spawnedPropCount * propFixedInterval - _totalMoveDistance;
            var randomOffset = Random.Range(-propPositionRandomRange, propPositionRandomRange);
            spawnPosX += randomOffset;
            
            var img = GetImageFromPool();
            img.sprite = sprite;
            img.SetNativeSize();
            
            var rect = img.rectTransform;
            var root = Random.Range(0f, 1f) < 0.7f ? backRoot : frontRoot;
            rect.SetParent(root, false);
            var width = sprite.rect.width;
            var height = sprite.rect.height;
            // 底部贴合地面
            rect.anchoredPosition = new Vector2(spawnPosX + width * 0.5f, groundHeight + height * 0.5f);
            _activeImages.Add(img);
            _spawnedPropCount++;
        }

        private void Movement()
        {
            // 更新滚动偏移
            var scrollDelta = scrollSpeed * Time.unscaledDeltaTime;
            _totalMoveDistance += scrollDelta;
            foreach (var activeImage in _activeImages)
            {
                activeImage.rectTransform.anchoredPosition += Vector2.left * scrollDelta;
            }
        }
        
        private void RecycleOutOfBoundsImages()
        {
            for (var i = _activeImages.Count - 1; i >= 0; i--)
            {
                var img = _activeImages[i];
                // 图片右边缘超出窗口左侧，回收
                if (img.rectTransform.anchoredPosition.x + img.rectTransform.sizeDelta.x * 0.5f < -10)
                {
                    ReturnImageToPool(img);
                    _activeImages.RemoveAt(i);
                }
            }
        }
        
        private Image GetImageFromPool()
        {
            if (_imagePool.Count > 0)
            {
                var img = _imagePool.Dequeue();
                img.gameObject.SetActive(true);
                return img;
            }
            
            // 创建新的Image
            var go = new GameObject("ScrollImage", typeof(Image));
            go.transform.SetParent(window, false);
            return go.GetComponent<Image>();
        }
        
        private void ReturnImageToPool(Image img)
        {
            img.sprite = null;
            img.gameObject.SetActive(false);
            _imagePool.Enqueue(img);
        }
        
        private void ReleaseAllImages()
        {
            foreach (var img in _activeImages)
            {
                ReturnImageToPool(img);
            }
            _activeImages.Clear();
        }
    }
}