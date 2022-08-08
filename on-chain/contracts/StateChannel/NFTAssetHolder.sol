// SPDX-License-Identifier: UNLICENSED
pragma solidity 0.7.6;
pragma experimental ABIEncoderV2;

import "./INFTAssetHolder.sol";
import {ExitFormat as Outcome} from '@statechannels/exit-format/contracts/ExitFormat.sol';
import '@statechannels/nitro-protocol/contracts/StatusManager.sol';
import '@openzeppelin/contracts/token/ERC1155/ERC1155Receiver.sol';
import '@openzeppelin/contracts/token/ERC1155/ERC1155.sol';

/**
 * @dev Adds functionality for storing NFTs against a channel
 */
contract NFTAssetHolder is INFTAssetHolder, StatusManager, ERC1155Receiver {

    // *******
    // Storage
    // *******

    address public collection;

    mapping(bytes32 => mapping(address => Batch)) nftHoldings;

    constructor(address collection) {
        collection = collection;
    }

    /**
     * @notice Transfers NFTs out of the channel back to the owner
     * @dev Transfers NFTs out of the channel back to the owner
     */
    function nft_reclaim(
        bytes32 fromChannelId,
        address owner
    ) external override {
        // ensure the channel is finalized
        require(_mode(fromChannelId) == ChannelMode.Finalized, "Cannot withdraw assets from an unfinalized channel");

        Batch storage batch = nftHoldings[fromChannelId][owner];
        ERC1155(collection).safeBatchTransferFrom(address(this), owner, batch.ids, batch.values, "");

        // emit event
        emit BatchReclaimed(fromChannelId, owner, batch.ids, batch.values);
    }

    /**
     * @notice Returns the batch of NFTs stored against the given channel by the owner
     * @dev Returns the batch of NFTs stored against the given channel by the owner
     */
    function stored_batch(// filter the batc
        bytes32 channelId,
        address owner
    ) external view override returns (Batch memory) {
        return nftHoldings[channelId][owner];
    }

    // do not allow non-batch deposits
    function onERC1155Received(
        address operator,
        address from,
        uint256 id,
        uint256 value,
        bytes calldata data
    ) public override returns (bytes4) {
        return 0;
    }

    function onERC1155BatchReceived(
        address operator,
        address from,
        uint256[] calldata ids,
        uint256[] calldata values,
        bytes calldata data
    ) public override returns (bytes4) {
        require(from == collection, "Contract can only hold NFTs from the collection specified in the constructor");
        // decode the metadata as the channel ID
        bytes32 channelId = toBytes32(data, 0);
        // require(nftHoldings[channelId][operator] == Batch(uint256[], uint256[]), "Caller already has an NFT batch stored against the channel. Withdraw these first");
        
        // store the batch against the channel pass in the data field
        nftHoldings[channelId][from] = Batch(ids, values);

        // emit event
        emit BatchDeposited(channelId, from, ids, values);

        return this.onERC1155BatchReceived.selector;
    }

    function toBytes32(bytes memory b, uint offset) private pure returns (bytes32) {
        bytes32 out;
        for (uint i = 0; i < 32; i++) {
            out |= bytes32(b[offset + i] & 0xFF) >> (i * 8);
        }
        return out;
    }
}
