using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainsManager {

    public class Node
    {
        public Arm arm = null;
        public Character character = null; // character head
        public GameObject obj = null;
        public List<Node> nodes = new List<Node>();

        public Node() {}

        public Node(Arm arm_)
        {
            arm = arm_;
        }

        public Node(Character character_)
        {
            character = character_;
        }

        public Node(GameObject obj_)
        {
            obj = obj_;
        }
    }

    List<Node> chains = new List<Node>();
    List<List<Node>> chainsUnderTension = new List<List<Node>>();

    public void ClearChains()
    {
        chains.Clear();
    }

    #region Handling types in nodes

    public Node GetNodeFromGameObject(GameObject hook)
    {
        if (hook.CompareTag("Character"))
        {
            Arm hookedArm = Arm.GetArmFromGameObject(hook);
            if (hookedArm != null)
            {
                return GetNode(hookedArm);
            }
            else
            {
                Character hookedCharacter = hook.GetComponent<Character>();
                if (hookedCharacter != null)
                {
                    return GetNode(hookedCharacter);
                }
                BodyPart bodyPart = hook.GetComponent<BodyPart>();
                if (bodyPart != null)
                {
                    return GetNode(bodyPart.arm);
                }
            }
        }
        else
        {
            return GetNode(hook);
        }

        return null;
    }

    Node CreateNodeFromGameObject(GameObject hook)
    {
        if (hook.CompareTag("Character"))
        {
            Arm hookedArm = Arm.GetArmFromGameObject(hook);
            if (hookedArm != null)
            {
                return new Node(hookedArm);
            }
            else
            {
                Character hookedCharacter = hook.GetComponent<Character>();
                if (hookedCharacter != null)
                {
                    return new Node(hookedCharacter);
                }
                else
                {
                    Debug.LogError("Unknown character object : " + hook.name);
                }
            }
        }
        return new Node(hook);
    }

    Character GetCharacterFromNode(Node node)
    {
        if (node.character != null)
        {
            return node.character;
        }
        else if (node.arm != null)
        {
            return node.arm.GetParentCharacter();
        }
        return null;
    }

    void AddAllNodesFromCharacter(Node node, Arm arm)
    {
        Node characterNode = new Node(arm.GetParentCharacter());
        node.nodes.Add(characterNode);
        characterNode.nodes.Add(node);

        if (arm.GetOtherArm() != null)
        {
            Node otherArmNode = new Node(arm.GetOtherArm());
            characterNode.nodes.Add(otherArmNode);
            otherArmNode.nodes.Add(characterNode);
        }
    }

    void AddAllNodesFromCharacter(Node node, Character character)
    {
        if (character.LeftArm != null)
        {
            Node leftArmNode = new Node(character.LeftArm);
            node.nodes.Add(leftArmNode);
            leftArmNode.nodes.Add(node);
        }
        if (character.RightArm != null)
        {
            Node rightArmNode = new Node(character.RightArm);
            node.nodes.Add(rightArmNode);
            rightArmNode.nodes.Add(node);
        }
    }

    #endregion

    public void AddHookToChain(Arm arm, GameObject hook)
    {
        Node node1 = GetNode(arm);
        Node node2 = GetNodeFromGameObject(hook);

        if (node1 != null && node1.nodes.Contains(node2) || node2 != null && node2.nodes.Contains(node1))
        {
            return;
        }

        if (node1 == null)
        {
            node1 = new Node(arm);
            AddAllNodesFromCharacter(node1, arm);
            chains.Add(node1);
        }
        if (node2 == null)
        {
            node2 = CreateNodeFromGameObject(hook);
            if (hook.CompareTag("Character"))
            {
                Arm hookedArm = Arm.GetArmFromGameObject(hook);
                if (hookedArm != null && hookedArm.GetParentCharacter() != null)
                {
                    AddAllNodesFromCharacter(node2, hookedArm);
                }
                else
                {
                    Character hookedCharacter = hook.GetComponent<Character>();
                    if (hookedCharacter != null)
                    {
                        AddAllNodesFromCharacter(node2, hookedCharacter);
                    }
                }
            }
            chains.Add(node2);
        }

        Node chain1 = GetChain(node1);
        Node chain2 = GetChain(node2);

        if (chain1 != chain2)
        {
            chains.Remove(chain2);
        }

        node1.nodes.Add(node2);
        node2.nodes.Add(node1);

        CheckIfNewChainRisksTension(chain1);
    }

    public void RemoveHookToChain(Arm arm, GameObject hook)
    {
        if (hook == null)
            return;

        Node node1 = GetNode(arm);
        Node node2 = GetNodeFromGameObject(hook);

        List<Node> chainUnderTension = GetChainUnderTension(node1);
        if (chainUnderTension != null && chainUnderTension.Count > 0)
        {
            StopChainUnderTension(chainUnderTension);
        }

        if (node1 == null || node2 == null)
            return;

        SplitChain(node1, node2);
    }

    void CheckIfNewChainRisksTension(Node chain)
    {
        List<Node> platformNodes = GetPlatformsConnected(chain);
        if (platformNodes.Count >= 2)
        {
            foreach (Node platformNode in platformNodes)
            {
                MovingPlatform movingPlatform = platformNode.obj.GetComponent<MovingPlatform>();
                if (movingPlatform != null)
                {
                    foreach (Node connectedNodes in platformNode.nodes)
                    {
                       // movingPlatform.StartCheckFixedJoint(connectedNodes.arm.GetHand());
                        PutChainUnderSurveillance(movingPlatform.gameObject);
                    }
                }
            }
        }
    }

    void FillPlatformList(Node newNode, ref List<Node> platforms, List<Node> checkedNodes)
    {
        checkedNodes.Add(newNode);
        if (newNode.obj != null && newNode.obj.CompareTag("Grab"))
        {
            platforms.Add(newNode);
        }
        foreach (Node node in newNode.nodes)
        {
            if (!checkedNodes.Contains(node))
            {
                FillPlatformList(node, ref platforms, checkedNodes);
            }
        }
    }

    List<Node> GetPlatformsConnected(Node newNode)
    {
        List<Node> platformNodes = new List<Node>();

        FillPlatformList(newNode, ref platformNodes, new List<Node>());
        return platformNodes;
    }

    public void SplitChain(Node node1, Node node2)
    {  
        if (node2.arm != null && node2.arm.GetParentCharacter() != null) // the other arm is still holding the one that released
        {
            Joint2D joint = node2.arm.GetHand().GetHook();
            if (joint != null)
            {
                Arm arm = Arm.GetArmFromGameObject(joint.gameObject);
                if (arm != null && arm == node1.arm)
                    return;
            }
        }

        node1.nodes.Remove(node2);
        node2.nodes.Remove(node1);

        Node chain1 = GetChain(node1);
        Node chain2 = GetChain(node2);

        Character characterNode2 = GetCharacterFromNode(node2);

        if (node1.nodes.Count <= 0 || HasOnlyNodesFromOwnCharacter(node1, node1.arm.GetParentCharacter(), new List<Node>()))
        {
            if (chain1 != null)
            {
                DeleteChain(chain1);
            }
            else
            {
                DeleteNodes(node1);
            }
        }
        else
        {
            if (chain1 == null)
            {
                chains.Add(node1);
            }
            if (!IsNodeUnderTension(node1))
            {
                CheckIfNewChainRisksTension(node1);
            }
        }

        if (node2.nodes.Count <= 0 || (characterNode2 != null && HasOnlyNodesFromOwnCharacter(node2, characterNode2, new List<Node>())))
        {
            if (chain2 != null)
            {
                DeleteChain(chain2);
            }
            else
            {
                DeleteNodes(node2);
            }
        }
        else
        {
            if (chain2 == null)
            {
                chains.Add(node2);
            }
            if (!IsNodeUnderTension(node2))
            {
                CheckIfNewChainRisksTension(node2);
            }
        }
    }

    public bool HasOnlyNodesFromOwnCharacter(Node node, Character character, List<Node> checkedNodes)
    {
        bool found = true;
        checkedNodes.Add(node);
        if ((node.arm != null && !character.IsGameObjectFromCharacter(node.arm.gameObject))
            || (node.character != null && character != node.character)
            || node.obj != null)
        {
            found = false;
        }
        else
        {
            foreach (Node n in node.nodes)
            {
                if (!checkedNodes.Contains(n))
                {
                    found = HasOnlyNodesFromOwnCharacter(n, character, checkedNodes);
                }
                if (!found)
                    return found;
            }
        }
        return found;
    }

    void DeleteChain(Node chain)
    {
        DeleteNodes(chain);
        chains.Remove(chain);
    }

    void DeleteNodes(Node node)
    {
        List<Node> nodesToDelete = new List<Node>();
        GetNodesInList(node, ref nodesToDelete);

        foreach (Node n in nodesToDelete)
        {
            n.arm = null;
            n.character = null;
            n.obj = null;
            n.nodes.Clear();
        }
    }

    void GetNodesInList(Node node, ref List<Node> checkedNodes)
    {
        foreach(Node n in node.nodes)
        {
            if (!checkedNodes.Contains(n))
            {
                checkedNodes.Add(n);
                GetNodesInList(n, ref checkedNodes);
            }
        }
    }

    #region Get nodes and chains

    Node GetChain(Character character)
    {
        foreach (Node chain in chains)
        {
            Node found = GetNodeInChain(chain, character, new List<Node>());
            if (found != null)
            {
                return chain;
            }
        }
        return null;
    }

    Node GetChain(Arm arm)
    {
        foreach (Node chain in chains)
        {
            Node found = GetNodeInChain(chain, arm, new List<Node>());
            if (found != null)
            {
                return chain;
            }
        }
        return null;
    }

    Node GetChain(GameObject obj)
    {
        foreach (Node chain in chains)
        {
            Node found = GetNodeInChain(chain, obj, new List<Node>());
            if (found != null)
            {
                return chain;
            }
        }
        return null;
    }

    Node GetChain(Node node)
    {
        foreach (Node chain in chains)
        {
            Node found = GetNodeInChain(chain, node, new List<Node>());
            if (found != null)
            {
                return chain;
            }
        }
        return null;
    }

    Node GetNodeInChain(Node chain, Character character, List<Node> checkedNodes)
    {
        Node found = null;
        checkedNodes.Add(chain);
        if (chain.character == character)
        {
            found = chain;
        }
        else if (chain.nodes.Count > 0)
        {
            foreach (Node node in chain.nodes)
            {
                if (!checkedNodes.Contains(node))
                {
                    found = GetNodeInChain(node, character, checkedNodes);
                    if (found != null)
                        return found;
                }
            }
        }
        return found;
    }

    Node GetNodeInChain(Node chain, Arm arm, List<Node> checkedNodes)
    {
        Node found = null;
        checkedNodes.Add(chain);
        if (chain.arm == arm)
        {
            found = chain;
        }
        else if (chain.nodes.Count > 0)
        {
            foreach (Node node in chain.nodes)
            {
                if (!checkedNodes.Contains(node))
                {
                    found = GetNodeInChain(node, arm, checkedNodes);
                    if (found != null)
                        return found;
                }
            }
        }
        return found;
    }

    Node GetNodeInChain(Node chain, GameObject obj, List<Node> checkedNodes)
    {
        Node found = null;
        checkedNodes.Add(chain);
        if (chain.obj == obj)
        {
            found = chain;
        }
        else if (chain.nodes.Count > 0)
        {
            foreach (Node node in chain.nodes)
            {
                if (!checkedNodes.Contains(node))
                {
                    found = GetNodeInChain(node, obj, checkedNodes);
                    if (found != null)
                        return found;
                }
            }
        }
        return found;
    }

    Node GetNodeInChain(Node chain, Node n, List<Node> checkedNodes)
    {
        Node found = null;
        checkedNodes.Add(chain);
        if (chain == n)
        {
            found = chain;
        }
        else if (chain.nodes.Count > 0)
        {
            foreach (Node node in chain.nodes)
            {
                if (!checkedNodes.Contains(node))
                {
                    found = GetNodeInChain(node, n, checkedNodes);
                    if (found != null)
                        return found;
                }
            }
        }
        return found;
    }

    Node GetNode(Character character)
    {
        foreach (Node chain in chains)
        {
            Node found = GetNodeInChain(chain, character, new List<Node>());
            if (found != null)
            {
                return found;
            }
        }
        return null;
    }

    Node GetNode(Arm arm)
    {
        foreach (Node chain in chains)
        {
            Node found = GetNodeInChain(chain, arm, new List<Node>());
            if (found != null)
            {
                return found;
            }
        }
        return null;
    }

    Node GetNode(GameObject obj)
    {
        foreach (Node chain in chains)
        {
            Node found = GetNodeInChain(chain, obj, new List<Node>());
            if (found != null)
            {
                return found;
            }
        }
        return null;
    }

    Node GetNode(Node node)
    {
        foreach (Node chain in chains)
        {
            Node found = GetNodeInChain(chain, node, new List<Node>());
            if (found != null)
            {
                return found;
            }
        }
        return null;
    }

    bool IsNodeUnderTension(Node node)
    {
        if (node == null)
        {
            Debug.Log("node null");
        }
        if (node.character != null)
        {
            return node.character.GetIsUnderTension();
        }
        else if (node.arm != null)
        {
            return node.arm.GetIsUnderTention();
        }
        return false;
    }

    public List<Node> GetChainUnderTension(Node node)
    {
        foreach (List<Node> chain in chainsUnderTension)
        {
            foreach (Node tensionNode in chain)
            {
                if (tensionNode == node)
                {
                    return chain;
                }
            }
        }
        return null;
    }

    #endregion

    public void PutChainUnderSurveillance(GameObject platform)
    {
        Node platformNode = GetNode(platform);
        List<Node> platformsNodes = GetPlatformsConnected(platformNode);
        platformsNodes.Remove(platformNode);

        List<Node> closestPlatformPath = null;
        int currentLengthPath = int.MaxValue;
        foreach (Node currentNode in platformsNodes)
        {
            List<Node> path = GetShortestPath(currentNode, platformNode);
            if (path.Count < currentLengthPath)
            {
                closestPlatformPath = path;
            }
        }

        if (closestPlatformPath == null || closestPlatformPath.Count == 0)
        {
            Debug.LogError("Error no path");
            return;
        }
        chainsUnderTension.Add(closestPlatformPath);
    }

    public void PutChainUnderTension(GameObject platform)
    {
        List<Node> chainUnderTension = GetChainUnderTension(GetNode(platform));

        foreach (Node node in chainUnderTension)
        {
            if (node.character != null)
            {
                node.character.SetIsUnderTension(true);
            }
            else if (node.arm != null)
            {
                node.arm.SetIsUnderTension(true);
                node.arm.GetParentCharacter().StuckVibrate(node.arm.IsLeft ? Character.eControllerMotors.LEFT : Character.eControllerMotors.RIGHT);
            }
        }
    }

    public void StopChainUnderTension(List<Node> chain)
    {
        List<Arm> arms = new List<Arm>();
        foreach (Node node in chain)
        {
            if (node.arm != null)
            {
                arms.Add(node.arm);
            }
        }

        foreach (Node node in chain)
        {
            if (node.character != null)
            {
                node.character.SetIsUnderTension(false);
            }
            else if (node.arm != null)
            {
                node.arm.SetIsUnderTension(false);
                if (node.arm.GetParentCharacter() != null)
                {
                    node.arm.GetParentCharacter().StopVibration();
                }
            }
            else
            {
                MovingPlatform movingPlatform = node.obj.GetComponent<MovingPlatform>();
                if (movingPlatform != null)
                {
                    foreach (Arm arm in arms)
                    {
                        if (arm.GetHand().GetIsHooked() && arm.GetHand().GetHook().gameObject == movingPlatform.gameObject)
                        {
                            //movingPlatform.StopCheckFixedJoint(arm.GetHand());
                        }
                    }
                }
            }
        }
        chainsUnderTension.Remove(chain);
    }

    public void BreakChainUnderTension(GameObject platform)
    {
        List<Node> chain = GetChainUnderTension(GetNode(platform));

        List<Arm> arms = new List<Arm>();
        Hand previousHand = null;
        foreach (Node node in chain)
        {
            if (node.character != null)
            {
                node.character.SetIsUnderTension(false);
            }
            else if (node.arm != null)
            {
                node.arm.SetIsUnderTension(false);
                node.arm.GetParentCharacter().BreakVibrate(node.arm.IsLeft ? Character.eControllerMotors.LEFT : Character.eControllerMotors.RIGHT);
                arms.Add(node.arm);
                previousHand = node.arm.GetHand();
            }
            else
            {
                MovingPlatform movingPlatform = node.obj.GetComponent<MovingPlatform>();
                if (movingPlatform != null)
                {
                    //movingPlatform.StopCheckFixedJoint(previousHand);
                }
            }
        }
        chainsUnderTension.Remove(chain);

        Arm armToBreak = arms[Random.Range(0, arms.Count)];
        SplitChain(GetNode(armToBreak), GetNode(armToBreak.GetParentCharacter()));

        armToBreak.Break();
    }

    bool IsChainEnchored(Node chain, Character startingCharacter, bool checkIsVisible, List<Node> checkedNodes)
    {
        bool isEnchored = false;
        checkedNodes.Add(chain);
        if (chain.character != null && chain.character.IsCharacterLayingOnGrabSurface() && (chain.character != startingCharacter || startingCharacter == null)
                || chain.obj != null && chain.obj.CompareTag("Grab"))
        {
            if (checkIsVisible && chain.obj != null)
            {
                if (LevelManager.IsObjectInsideCamera(chain.obj.GetComponent<SpriteRenderer>()))
                {
                    return true;
                }
                else if (chain.obj.name.Contains("Node")) // Used for ropes
                {
                    OptimizerHideObjects rope = chain.obj.GetComponentInParent<OptimizerHideObjects>();
                    if (rope != null
                        && ((rope.GetStartRigidbody().bodyType == RigidbodyType2D.Dynamic) && LevelManager.IsObjectInsideCamera(rope.Start)
                        || (rope.GetEndRigidbody().bodyType == RigidbodyType2D.Dynamic) && LevelManager.IsObjectInsideCamera(rope.End)))
                    {
                        return true;
                    }
                }
            }
            else if (!checkIsVisible)
            {
                return true;
            }
        }
        foreach (Node node in chain.nodes)
        {
            if (!checkedNodes.Contains(node))
            {
                isEnchored = IsChainEnchored(node, startingCharacter, checkIsVisible, checkedNodes);
                if (isEnchored)
                    return isEnchored;
            }
        }
        return isEnchored;
    }

    public bool IsHookEnchored(Arm arm)
    {
        if (!arm.GetHand().GetIsHooked())
            return false;

        if (arm.GetHand().GetIsHooked() && arm.GetHand().GetHook().CompareTag("Grab"))
        {
            return true;
        }
        Node chain = GetNodeFromGameObject(arm.GetHand().GetHook().gameObject);
        return (chain != null && IsChainEnchored(chain, arm.GetParentCharacter(), false, new List<Node>() { GetNode(arm) }));
    }

    public bool IsEnchored(Arm arm)
    {
        if (!arm.GetHand().GetIsHooked())
            return false;

        if (arm.GetHand().GetIsHooked() && arm.GetHand().GetHook().CompareTag("Grab"))
        {
            return true;
        }
        Node chain = GetNode(arm);
        return (chain != null && IsChainEnchored(chain, arm.GetParentCharacter(), false, new List<Node>() { GetNode(arm) }));
    }

    public bool IsAtLeatOneEnchored(List<Arm> arms)
    {
        foreach (Arm arm in arms)
        {
            if (IsEnchored(arm))
            {
                return true;
            }
        }
        return false;
    }

    /*bool IsChainEnchoredVisible(Node chain, List<Node> checkedNodes)
    {
        bool isEnchored = false;
        checkedNodes.Add(chain);
        if (chain.obj != null && chain.obj.CompareTag("Grab") && LevelManager.IsObjectInsideCamera(chain.obj))
        {
            return true;
        }
        foreach (Node node in chain.nodes)
        {
            if (!checkedNodes.Contains(node))
            {
                isEnchored = IsChainEnchored(node, null, checkedNodes);
                if (isEnchored)
                    return isEnchored;
            }
        }
        return isEnchored;
    }*/

    public bool IsEnchoredVisible(Arm arm)
    {
        if (!arm.GetHand().GetIsHooked())
            return false;

        if (arm.GetHand().GetIsHooked() && arm.GetHand().GetHook().CompareTag("Grab"))
        {
            if (LevelManager.IsObjectInsideCamera(arm.GetHand().GetHook().gameObject))
            {
                return true;
            }
            else if (arm.GetHand().GetHook().name.Contains("Node")) // Used for ropes
            {
                OptimizerHideObjects rope = arm.GetHand().GetHook().GetComponentInParent<OptimizerHideObjects>();
                if (rope != null
                    && ((rope.GetStartRigidbody().bodyType == RigidbodyType2D.Static) && LevelManager.IsObjectInsideCamera(rope.Start.GetComponent<SpriteRenderer>())
                    || (rope.GetEndRigidbody().bodyType == RigidbodyType2D.Static) && LevelManager.IsObjectInsideCamera(rope.End.GetComponent<SpriteRenderer>())))
                {
                    return true;
                }
            }
        }
        Node chain = GetNodeFromGameObject(arm.GetHand().GetHook().gameObject);
        return (chain != null && IsChainEnchored(chain, arm.GetParentCharacter(), true, new List<Node>() { GetNode(arm) }));
    }

    public bool IsEnchoredVisible(Character character)
    {
        Node chain = GetNode(character);
        return (chain != null && IsChainEnchored(chain, null, true, new List<Node>()));
    }

    public void GetCharactersInChain(GameObject obj, List<Node> checkedNodes, ref List<Character> characters, bool stopsWhenNodeIsNotCharacter = false)
    {
        Node node = GetNodeFromGameObject(obj);
        if (node == null)
            return;
        GetCharactersInChainFromNode(node, checkedNodes, ref characters, stopsWhenNodeIsNotCharacter);
    }

    public Node GetNodeGameObjectInCharacterChain(Character characterFromChain, GameObject obj)
    {
        Node chain = GetChain(characterFromChain);
        if (chain == null)
            return null;
        return GetNodeInChain(chain, obj, new List<Node>());
    }

    void GetCharactersInChainFromNode(Node chain, List<Node> checkedNodes, ref List<Character> characters, bool stopsWhenNodeIsPlatform = false)
    {
        checkedNodes.Add(chain);
        foreach (Node node in chain.nodes)
        {
            if (!checkedNodes.Contains(node) && (!stopsWhenNodeIsPlatform || (stopsWhenNodeIsPlatform && (node.obj == null || !node.obj.CompareTag("Grab"))))) // if Node is not a GameObject or a platform
            {
                if (node.character != null && !characters.Contains(node.character))
                {
                    characters.Add(node.character);
                }
                GetCharactersInChainFromNode(node, checkedNodes, ref characters, stopsWhenNodeIsPlatform);
            }
        }
    }

    public List<Node> GetShortestPath(Node start, Node end)
    {
        Dictionary<Node, Node> previous = new Dictionary<Node, Node>();

        Queue<Node> queue = new Queue<Node>();
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            Node vertex = queue.Dequeue();
            foreach (Node neighbor in vertex.nodes)
            {
                if (previous.ContainsKey(neighbor))
                    continue;

                previous[neighbor] = vertex;
                queue.Enqueue(neighbor);
            }
        }

        List<Node> path = new List<Node> { };

        Node current = end;
        while (!current.Equals(start))
        {
            path.Add(current);
            current = previous[current];
        };

        path.Add(start);
        path.Reverse();

        return path;
    }
}
