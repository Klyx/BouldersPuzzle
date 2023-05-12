using System;
using System.Collections.Generic;
using System.Globalization;
using FG.Gridmap;
using UIWidgets;
using UnityEngine;

namespace FG {
    [DefaultExecutionOrder(100)]
    [RequireComponent(typeof(TreeViewDataSource))]
    public class EditorLayers : MonoBehaviour {
        [SerializeField] private Sprite[] _icons = new Sprite[] { };
        [SerializeField] private BlockCount _blockCount;

        private TreeView _tree;
        private ObservableList<TreeNode<TreeViewItem>> _nodes;

        public void AddNodesFromMap(GridMap map) {
            map.GetAllBlocks(out List<Block> blocks);
            foreach (Block block in blocks) {
                TreeNode<TreeViewItem> rootNode = null;
                foreach (TreeNode<TreeViewItem> node in _nodes) {
                    if (((int)node.Item.Tag) == block.PositionInt.y) {
                        rootNode = node;
                        break;
                    }
                }
                
                if (ReferenceEquals(rootNode, null)) {
                    TreeViewItem rootTreeViewItem =
                        new TreeViewItem(block.PositionInt.y.ToString(NumberFormatInfo.InvariantInfo));
                    rootTreeViewItem.Tag = block.PositionInt.y;
                    rootNode = new TreeNode<TreeViewItem>(rootTreeViewItem, new ObservableList<TreeNode<TreeViewItem>>());
                    _nodes.Add(rootNode);
                    _tree.SelectNode(rootNode);
                }

                Localization.Instance.GetText(block.BlockName, out string blockName);
                TreeViewItem treeViewItem = new TreeViewItem(blockName, _icons[block.PrefabIndex]);
                treeViewItem.Tag = block;
                TreeNode<TreeViewItem> treeNode = new TreeNode<TreeViewItem>(treeViewItem);
                rootNode.Nodes.Add(treeNode);
                rootNode.IsExpanded = true;
                _tree.SelectNode(treeNode);
            }
            
            _blockCount.SetBlockCount(map.GetBlockCount());
            SortAsc();
        }

        public void ClearNodes() {
            _tree.Clear();
        }

        public void OnBlockChanged(GridMap map, Block block, bool added, Vector3Int position) {
            if (added) {
                TreeNode<TreeViewItem> rootNode = null;
                foreach (TreeNode<TreeViewItem> node in _nodes) {
                    if (((int)node.Item.Tag) == block.PositionInt.y) {
                        rootNode = node;
                        break;
                    }
                }
                
                if (ReferenceEquals(rootNode, null)) {
                    TreeViewItem rootTreeViewItem =
                        new TreeViewItem(block.PositionInt.y.ToString(NumberFormatInfo.InvariantInfo));
                    rootTreeViewItem.Tag = block.PositionInt.y;
                    rootNode = new TreeNode<TreeViewItem>(rootTreeViewItem, new ObservableList<TreeNode<TreeViewItem>>());
                    _nodes.Add(rootNode);
                    _tree.SelectNode(rootNode);
                }

                Localization.Instance.GetText(block.BlockName, out string blockName);
                TreeViewItem treeViewItem = new TreeViewItem(blockName, _icons[block.PrefabIndex]);
                treeViewItem.Tag = block;
                TreeNode<TreeViewItem> treeNode = new TreeNode<TreeViewItem>(treeViewItem);
                rootNode.Nodes.Add(treeNode);
                rootNode.IsExpanded = true;
                _tree.SelectNode(treeNode);
            }
            else if(!ReferenceEquals(block, null)) {
                TreeNode<TreeViewItem> rootNode = _nodes.Find(rootItem =>
                    rootItem.Item.Name.Equals(block.PositionInt.y.ToString(CultureInfo.InvariantCulture)));
                TreeNode<TreeViewItem> treeNode =
                    rootNode.Nodes.Find(item => ((Block)item.Item.Tag).PositionInt.Equals(position));
                if (treeNode != null) {
                    rootNode.Nodes.Remove(treeNode);
                    if (rootNode.Nodes.Count == 0) {
                        _nodes.Remove(rootNode);
                    }
                }
            }
            
            SortAsc();
        }

        private void NodeSelected(TreeNode<TreeViewItem> node)
        {
            if (node.Item.Tag is int) {
                node.IsExpanded = true;
                foreach (TreeNode<TreeViewItem> blockNode in node.Nodes) {
                    Block block = (Block)blockNode.Item.Tag;
                    if (!ReferenceEquals(block, null) && _tree.IsSelected(blockNode.Index)) {
                        block.gameObject.SetActive(true);
                    }
                }
            }
            else if (node.Item.Tag is Block) {
                Block block = (Block)node.Item.Tag;
                if (!ReferenceEquals(block, null)) {
                    block.gameObject.SetActive(true);
                }
            }
        }
        
        private void NodeDeselected(TreeNode<TreeViewItem> node)
        {
            if (node.Item.Tag is int) {
                foreach (TreeNode<TreeViewItem> blockNode in node.Nodes) {
                    Block block = (Block)blockNode.Item.Tag;
                    if (!ReferenceEquals(block, null)) {
                        block.gameObject.SetActive(false);
                    }
                }
            }
            else if (node.Item.Tag is Block) {
                Block block = (Block)node.Item.Tag;
                if (!ReferenceEquals(block, null)) {
                    block.gameObject.SetActive(false);
                }
            }
        }
        
        Comparison<TreeNode<TreeViewItem>> comparisonAsc = (x, y) =>
            x.Item.Tag is int ? ((int)x.Item.Tag).CompareTo((int)y.Item.Tag) : (String.Compare(y.Item.Name, y.Item.Name, StringComparison.Ordinal));
        
        private void SortAsc()
        {
            _nodes.BeginUpdate();
            ApplyNodesSort(_nodes, comparisonAsc);
            _nodes.EndUpdate();
        }
        
        private void ApplyNodesSort<T>(ObservableList<TreeNode<T>> nodes, Comparison<TreeNode<T>> comparison)
        {
            // apply sort for current nodes
            nodes.Sort(comparison);
            // apply sort for child nodes
            nodes.ForEach(node =>
            {
                if (node.Nodes != null)
                {
                    ApplyNodesSort(node.Nodes as ObservableList<TreeNode<T>>, comparison);
                }
            });
        }

        private void Start() {
            _nodes = _tree.Nodes;
            _tree.NodeSelected.AddListener(NodeSelected);
            _tree.NodeDeselected.AddListener(NodeDeselected);
        }

        private void Awake() {
            _tree = GetComponent<TreeView>();
        }
    }
}