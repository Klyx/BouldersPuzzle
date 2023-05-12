using System;
using System.Collections.Generic;
using FG.Gridmap;
using UnityEngine;

namespace FG {
    public static class MovementUtility {
        public static void ReorderPlayerBlocks(ref List<PlayerBlock> playerBlocks, Transform playerTransform,
            Transform cameraTransform, in Bounds bounds) {
            if (playerBlocks.Count <= 1) {
                return;
            }

            Vector3 playerPosition = playerTransform.position.Flattened();
            Vector3 cameraPosition = cameraTransform.position.Flattened();
            Vector3 cameraPlayerDirection = cameraPosition.DirectionTo(playerPosition).ToVector3Int();
            Vector3 playerBlocksDirection = (playerBlocks[0].PositionInt - playerBlocks[1].PositionInt);
            playerBlocksDirection.Normalize();

            if (Mathf.Abs(Vector3.Dot(playerBlocksDirection, Vector3.up)) > 0.9f) {
                playerBlocks.Sort(new DistanceComparer(bounds.max));
            }
            else {
                if (cameraPlayerDirection == Vector3.forward) {
                    if (Mathf.Abs(Vector3.Dot(playerBlocksDirection, Vector3.forward)) > 0.9f) {
                        playerBlocks.Sort(new DistanceComparer(bounds.max));
                    }
                    else if (Mathf.Abs(Vector3.Dot(playerBlocksDirection, Vector3.right)) > 0.9f) {
                        playerBlocks.Sort(new DistanceComparer(bounds.min));
                    }
                }
                else if (cameraPlayerDirection == Vector3.back) {
                    if (Mathf.Abs(Vector3.Dot(playerBlocksDirection, Vector3.forward)) > 0.9f) {
                        playerBlocks.Sort(new DistanceComparer(bounds.min));
                    }
                    else if (Mathf.Abs(Vector3.Dot(playerBlocksDirection, Vector3.right)) > 0.9f) {
                        playerBlocks.Sort(new DistanceComparer(bounds.max));
                    }
                }
                else if (cameraPlayerDirection == Vector3.right) {
                    if (Mathf.Abs(Vector3.Dot(playerBlocksDirection, Vector3.right)) > 0.9f) {
                        playerBlocks.Sort(new DistanceComparer(bounds.max));
                    }
                    else if (Mathf.Abs(Vector3.Dot(playerBlocksDirection, Vector3.forward)) > 0.9f) {
                        playerBlocks.Sort(new DistanceComparer(bounds.max));
                    }
                }
                else if (cameraPlayerDirection == Vector3.left) {
                    if (Mathf.Abs(Vector3.Dot(playerBlocksDirection, Vector3.left)) > 0.9f) {
                        playerBlocks.Sort(new DistanceComparer(bounds.min));
                    }
                    else if (Mathf.Abs(Vector3.Dot(playerBlocksDirection, Vector3.forward)) > 0.9f) {
                        playerBlocks.Sort(new DistanceComparer(bounds.min));
                    }
                }
            }
        }

        public static bool StandsOnValidBlock(ref List<PlayerBlock> playerBlocks, GridMap gridMap) {
            gridMap.GetBlock(playerBlocks[playerBlocks.Count - 1].PositionInt + Vector3Int.down, out Block block);
            if (ReferenceEquals(block, null) || block is DuplicatorBlock) {
                return false;
            }

            return true;
            //return gridMap.IsOccupied(playerBlocks[playerBlocks.Count - 1].PositionInt + Vector3Int.down);
        }

        public static void ConnectWithBlocks(Transform cameraTransform, ref List<PlayerBlock> playerBlocks,
            GridMap gridMap) {
            Vector3Int cameraForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).ToVector3Int();
            Vector3Int cameraRight = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).ToVector3Int();

            List<PlayerBlock> playerBlocksToRemove = new List<PlayerBlock>(playerBlocks.Count);
            Vector3Int previousPlayerBlockTurnedPosition = Vector3Int.zero;

            for (int i = 0; i < playerBlocks.Count; i++) {
                Vector3Int blockPosition = playerBlocks[i].transform.position.ToVector3Int();
                Vector3Int[] nearbyBlockPositions = new[] {
                    blockPosition + cameraForward, blockPosition + -cameraForward,
                    blockPosition + -cameraRight, blockPosition + cameraRight,
                    blockPosition + Vector3Int.up, blockPosition + Vector3Int.down
                };

                gridMap.GetBlocks(in nearbyBlockPositions, out Block[] nearbyBlocks);
                Block[] nearbyBlocksSorted = new Block[nearbyBlocks.Length];
                Array.Copy(nearbyBlocks, nearbyBlocksSorted, nearbyBlocks.Length);
                GridMapUtility.SortBlocksByPriority(ref nearbyBlocksSorted);

                Vector3Int playerBlockDirection = Vector3Int.zero;
                if (playerBlocks.Count > 1) {
                    playerBlockDirection = playerBlocks[0].PositionInt - playerBlocks[1].PositionInt;
                }


                if (!ReferenceEquals(nearbyBlocksSorted[0], null)) {
                    if (!ReferenceEquals(nearbyBlocksSorted[1], null) &&
                        nearbyBlocksSorted[0].PositionInt.Equals(previousPlayerBlockTurnedPosition)) {
                        nearbyBlocksSorted[0] = nearbyBlocksSorted[1];
                    }

                    // if (nearbyBlocksSorted[0].DisruptConnection || nearbyBlocksSorted[0] is GoalBlock) {
                    //     if (nearbyBlocksSorted[0].ConnectWithPlayer(playerBlocks[i], gridMap)) {
                    //         playerBlocksToRemove.Add(playerBlocks[i]);
                    //         previousPlayerBlockTurnedPosition = playerBlocks[i].PositionInt;
                    //     }
                    // }

                    if (nearbyBlocksSorted[0] is GoalBlock) {
                        if (nearbyBlocksSorted[0].ConnectWithPlayer(playerBlocks[i], gridMap)) {
                            playerBlocksToRemove.Add(playerBlocks[i]);
                            previousPlayerBlockTurnedPosition = playerBlocks[i].PositionInt;
                        }
                    }

                    if (nearbyBlocksSorted[0].DisruptConnection) {
                        if (nearbyBlocksSorted[0].ConnectWithPlayer(playerBlocks[i], gridMap)) {
                            for (int k = i; k < playerBlocks.Count; k++) {
                                playerBlocksToRemove.Add(playerBlocks[k]);
                            }
                            //playerBlocksToRemove.Add(playerBlocks[i]);
                            previousPlayerBlockTurnedPosition = playerBlocks[i].PositionInt;
                        }
                    }
                    else {
                        // 0 - forward
                        // 1 - backwards
                        // 2 - left
                        // 3 - right
                        // 4 - up
                        // 5 - down

                        // Sideways
                        if (playerBlocks.Count == 1 ||
                            Mathf.Abs(Vector3.Dot(playerBlockDirection, cameraRight)) > 0.9f) {
                            ConnectPlayerToPlayer(nearbyBlocks[2], nearbyBlocks[3], ref playerBlocks, gridMap);
                        }

                        if (playerBlocks.Count == 1 ||
                            Mathf.Abs(Vector3.Dot(playerBlockDirection, Vector3.up)) > 0.9f) {
                            ConnectPlayerToPlayer(nearbyBlocks[4], nearbyBlocks[5], ref playerBlocks, gridMap);
                        }

                        if (playerBlocks.Count == 1 ||
                            Mathf.Abs(Vector3.Dot(playerBlockDirection, cameraForward)) > 0.9f) {
                            ConnectPlayerToPlayer(nearbyBlocks[0], nearbyBlocks[1], ref playerBlocks, gridMap);
                        }
                    }
                }
            }

            foreach (PlayerBlock block in playerBlocksToRemove) {
                GameObject.Destroy(block.gameObject);
                playerBlocks.Remove(block);
                //gridMap.RemoveBlock(block.PositionInt, true);
            }
        }

        private static bool ConnectPlayerToPlayer(Block blockA, Block blockB, ref List<PlayerBlock> playerBlocks,
            GridMap gridMap) {
            bool didAddBlock = false;

            if (!ReferenceEquals(blockA, null) && blockA is PlayerBlock) {
                blockA.ConnectWithPlayer(null, gridMap);
                AddBlock(blockA, ref playerBlocks);
                didAddBlock = true;
            }

            if (!ReferenceEquals(blockB, null) && blockB is PlayerBlock) {
                blockB.ConnectWithPlayer(null, gridMap);
                AddBlock(blockB, ref playerBlocks);
                didAddBlock = true;
            }

            void AddBlock(Block block, ref List<PlayerBlock> blocks) {
                gridMap.RemoveBlock(block.PositionInt);
                blocks.Add((PlayerBlock)block);
            }

            return didAddBlock;
        }

        /// <summary>
        /// Returns true if blocks left to move.
        /// </summary>
        public static bool UpdateMovablePlayerBlocks(Vector3Int moveDirection, GridMap gridMap,
            ref List<PlayerBlock> playerBlocks) {
            if (playerBlocks.Count == 1) {
                if (!gridMap.IsOccupied(playerBlocks[0].PositionInt + moveDirection) &&
                    gridMap.IsOccupied(playerBlocks[0].PositionInt + moveDirection + Vector3Int.down)) {
                    return true;
                }
            }

            Vector3Int playerBlockDirection = Vector3Int.zero;
            if (playerBlocks.Count > 1) {
                playerBlockDirection = playerBlocks[0].PositionInt - playerBlocks[1].PositionInt;
            }

            int firstBlockIndex = -1;
            int lastBlockIndex = -1;

            if (Mathf.Abs(Vector3.Dot(playerBlockDirection, Vector3.up)) > 0.9f) {
                for (int row = 0; row < playerBlocks.Count; row++) {
                    for (int y = 0; y <= row; y++) {
                        if (CanPlaceOnRow(playerBlocks[y].PositionInt, row + 1)) {
                            lastBlockIndex = y;
                        }
                    }

                    if (lastBlockIndex >= 0) {
                        break;
                    }
                }

                if (lastBlockIndex == playerBlocks.Count - 1) {
                    foreach (Block block in playerBlocks) {
                        for (int column = 1; column <= playerBlocks.Count; column++) {
                            Vector3Int position = block.PositionInt + (moveDirection * column);
                            if (gridMap.IsOccupied(position)) {
                                return false;
                            }
                        }
                    }
                }

                bool CanPlaceOnRow(Vector3Int blockPosition, int checkSlots) {
                    for (int z = 0; z < checkSlots; z++) {
                        Vector3Int forwardBlockPosition = blockPosition + (moveDirection * (z + 1));
                        Vector3Int forwardDownBlockPosition = forwardBlockPosition + Vector3Int.down;

                        if (gridMap.IsOccupied(forwardBlockPosition) ||
                            !gridMap.IsOccupied(forwardDownBlockPosition)) {
                            return false;
                        }
                    }

                    return true;
                }
            }
            else if (Mathf.Abs(Vector3.Dot(playerBlockDirection, Vector3.right)) > 0.9f) {
                if (Mathf.Abs(Vector3.Dot(moveDirection, Vector3.forward)) > 0.9f) {
                    for (int x = 0; x < playerBlocks.Count; x++) {
                        Vector3Int forwardBlockPosition = playerBlocks[x].PositionInt + moveDirection;
                        Vector3Int forwardDownBlockPosition = forwardBlockPosition + Vector3Int.down;

                        if (!gridMap.IsOccupied(forwardBlockPosition) && gridMap.IsOccupied(forwardDownBlockPosition)) {
                            firstBlockIndex = x;
                            break;
                        }
                    }

                    if (firstBlockIndex >= 0) {
                        for (int x = firstBlockIndex; x < playerBlocks.Count; x++) {
                            Vector3Int forwardBlockPosition = playerBlocks[x].PositionInt + moveDirection;
                            Vector3Int forwardDownBlockPosition = forwardBlockPosition + Vector3Int.down;

                            if (!gridMap.IsOccupied(forwardBlockPosition) &&
                                gridMap.IsOccupied(forwardDownBlockPosition)) {
                                lastBlockIndex = x;
                            }
                            else {
                                break;
                            }
                        }
                    }
                }
                else {
                    Block rotateAroundBlock = playerBlocks[0];
                    if ((Vector3.Dot(moveDirection, Vector3.right) > 0.9f &&
                         Vector3.Dot(playerBlockDirection, Vector3.left) > 0.9f) ||
                        (Vector3.Dot(moveDirection, Vector3.left) > 0.9f &&
                         Vector3.Dot(playerBlockDirection, Vector3.right) > 0.9f)) {
                        rotateAroundBlock = playerBlocks[playerBlocks.Count - 1];
                    }

                    for (int y = 0; y < playerBlocks.Count; y++) {
                        Vector3Int sideBlockPosition =
                            rotateAroundBlock.PositionInt + new Vector3Int(0, y, 0) + moveDirection;
                        if (gridMap.IsOccupied(sideBlockPosition)) {
                            return false;
                        }
                    }

                    foreach (Block block in playerBlocks) {
                        for (int y = 1; y < playerBlocks.Count; y++) {
                            Vector3Int sideBlockPosition =
                                block.PositionInt + new Vector3Int(0, y, 0);
                            if (gridMap.IsOccupied(sideBlockPosition)) {
                                return false;
                            }
                        }
                    }

                    return gridMap.IsOccupied(rotateAroundBlock.PositionInt + moveDirection + Vector3Int.down);
                }
            }
            else if (Mathf.Abs(Vector3.Dot(playerBlockDirection, Vector3.forward)) > 0.9f) {
                if (Mathf.Abs(Vector3.Dot(moveDirection, Vector3.right)) > 0.9f) {
                    for (int x = 0; x < playerBlocks.Count; x++) {
                        Vector3Int sideBlockPosition = playerBlocks[x].PositionInt + moveDirection;
                        Vector3Int sideDownBlockPosition = sideBlockPosition + Vector3Int.down;

                        if (!gridMap.IsOccupied(sideBlockPosition) && gridMap.IsOccupied(sideDownBlockPosition)) {
                            firstBlockIndex = x;
                            break;
                        }
                    }

                    lastBlockIndex = firstBlockIndex;
                    for (int x = firstBlockIndex + 1; x < playerBlocks.Count; x++) {
                        Vector3Int sideBlockPosition = playerBlocks[x].PositionInt + moveDirection;
                        Vector3Int sideDownBlockPosition = sideBlockPosition + Vector3Int.down;

                        if (!gridMap.IsOccupied(sideBlockPosition) && gridMap.IsOccupied(sideDownBlockPosition)) {
                            lastBlockIndex = x;
                        }
                        else {
                            break;
                        }
                    }
                }
                else if (Mathf.Abs(Vector3.Dot(moveDirection, Vector3.forward)) > 0.9f) {
                    Block rotateAroundBlock = playerBlocks[0];
                    if ((Vector3.Dot(moveDirection, Vector3.forward) > 0.9f &&
                         Vector3.Dot(playerBlockDirection, Vector3.back) > 0.9f) ||
                        (Vector3.Dot(moveDirection, Vector3.back) > 0.9f &&
                         Vector3.Dot(playerBlockDirection, Vector3.forward) > 0.9f)) {
                        rotateAroundBlock = playerBlocks[playerBlocks.Count - 1];
                    }

                    for (int y = 0; y < playerBlocks.Count; y++) {
                        Vector3Int sideBlockPosition =
                            rotateAroundBlock.PositionInt + new Vector3Int(0, y, 0) + moveDirection;

                        if (gridMap.IsOccupied(sideBlockPosition)) {
                            return false;
                        }
                    }

                    foreach (Block block in playerBlocks) {
                        for (int y = 1; y < playerBlocks.Count; y++) {
                            Vector3Int sideBlockPosition =
                                block.PositionInt + new Vector3Int(0, y, 0);
                            if (gridMap.IsOccupied(sideBlockPosition)) {
                                return false;
                            }
                        }
                    }

                    return gridMap.IsOccupied(rotateAroundBlock.PositionInt + moveDirection + Vector3Int.down);
                }
            }

            if (firstBlockIndex < 0 && lastBlockIndex < 0) {
                return false;
            }

            if (lastBlockIndex >= firstBlockIndex && lastBlockIndex < playerBlocks.Count - 1) {
                for (int i = lastBlockIndex + 1; i < playerBlocks.Count; i++) {
                    playerBlocks[i].DisconnectFromPlayer(i, gridMap);
                    DisconnectPlayerFromGoalBlocks(i, playerBlocks[i], gridMap);
                }

                playerBlocks.RemoveRange(lastBlockIndex + 1, playerBlocks.Count - lastBlockIndex - 1);
            }

            if (firstBlockIndex > 0) {
                for (int i = 0; i < firstBlockIndex; i++) {
                    playerBlocks[i].DisconnectFromPlayer(i, gridMap);
                    DisconnectPlayerFromGoalBlocks(i, playerBlocks[i], gridMap);
                }

                playerBlocks.RemoveRange(0, firstBlockIndex);
            }

            return true;
        }

        public static void DisconnectPlayerFromGoalBlocks(int index, PlayerBlock playerBlock, GridMap gridMap) {
            Vector3Int positionInt = playerBlock.PositionInt;
            Vector3Int[] positions = new[] {
                positionInt + Vector3Int.forward, positionInt + Vector3Int.back,
                positionInt + Vector3Int.right, positionInt + Vector3Int.left,
                positionInt + Vector3Int.up, positionInt + Vector3Int.down
            };
            gridMap.GetBlocks(in positions, out Block[] nearbyBlocks);

            foreach (Block block in nearbyBlocks) {
                GoalBlock goalBlock = block as GoalBlock;
                if (!ReferenceEquals(goalBlock, null)) {
                    goalBlock.DisconnectFromPlayer(index, gridMap);
                }
            }
        }

        public static void GetTotalRenderBounds(ref List<PlayerBlock> blocks, out Bounds bounds) {
            if (blocks.Count == 0) {
                bounds = new Bounds();
                return;
            }

            bounds = new Bounds(blocks[0].PositionInt, Vector3.one);
            for (int i = 1; i < blocks.Count; i++) {
                bounds.Encapsulate(blocks[i].GetComponent<Renderer>().bounds);
            }
        }

        public static Vector3 GetRotationPoint(Vector3Int moveWorldDirection, in Bounds bounds) {
            Vector3 point = Vector3Int.zero;

            // == uses Approximately on Vectors
            if (moveWorldDirection == Vector3Int.forward) {
                point.Set(0, bounds.min.y, bounds.max.z);
            }
            else if (moveWorldDirection == Vector3Int.back) {
                point.Set(0, bounds.min.y, bounds.min.z);
            }
            else if (moveWorldDirection == Vector3Int.right) {
                point.Set(bounds.max.x, bounds.min.y, 0);
            }
            else if (moveWorldDirection == Vector3Int.left) {
                point.Set(bounds.min.x, bounds.min.y, 0);
            }

            return point;
        }
    }
}