// SPDX-License-Identifier: MIT

/**
 * Created on 2022-06-23 12:37
 * @summary: Registry stores a mapping from IDs to contract addresses for various moves, items and status conditions
 * It also handles delegating the calls and ensures calls to non-existant contracts is a NOOP
 * @author: Willem Olding (ChainSafe)
 */
 pragma solidity 0.7.6;
 pragma experimental ABIEncoderV2;

 import "@openzeppelin/contracts/access/AccessControl.sol";

 import { ToshimonState as TM } from './ToshimonState.sol';
 import './interfaces/IMove.sol';
 import './interfaces/IItem.sol';


 contract ToshimonRegistry is AccessControl {
    bytes32 public constant MANAGER_ROLE = keccak256("MANAGER_ROLE");

    // Moves use their move number to index
    mapping(uint32 => address) public moves;

    // Stems should use their tokenId in the base ERC1155 contract
    mapping(uint32 => address) public items;

    // Status conditions should use their index number 
    mapping(uint32 => address) public statusConditions;


    constructor() {
        // Grant the contract deployer the default admin role: it will be able
        // to grant and revoke any roles
        _setupRole(DEFAULT_ADMIN_ROLE, msg.sender);
        // also make the deployer a manager of the content
        _setupRole(MANAGER_ROLE, msg.sender);
    }

    /**
     * Allows a manager of the registry to add a new move to the mapping
     */
    function addMove(uint32 index, address contractAddress) public {
        require(hasRole(MANAGER_ROLE, msg.sender), "Caller is not a manager of the registry");
        moves[index] = contractAddress;
    }

    /**
     * Tries to call an external contract implementing a move.
     * This can fail if either:
     *     - The move index does not exist in the registry
     *     - The contract at the move address does not implement IMove
     *     - The implementation of applyMove throws an error
     */
    function callMove(uint32 index, TM.GameState memory state, uint8 mover, bytes32 randomSeed) public returns (TM.GameState memory) {
        return IMove(moves[index]).applyMove(state, mover, randomSeed);
    }
 
    /**
     * Allows a manager of the registry to add a new item to the mapping
     */
    function addItem(uint32 index, address contractAddress) public {
        require(hasRole(MANAGER_ROLE, msg.sender), "Caller is not a manager of the registry");
        items[index] = contractAddress;
    }

    /**
     * Tries to call an external contract implementing a item.
     * This can fail if either:
     *     - The item index does not exist in the registry
     *     - The contract at the item address does not implement IMove
     *     - The implementation of applyMove throws an error
     */
    function callItem(uint32 index, TM.GameState memory state, uint8 mover, uint8 monster, bytes32 randomSeed) public returns (TM.GameState memory) {
        return IItem(moves[index]).applyItem(state, mover, monster, randomSeed);
    }    
}
