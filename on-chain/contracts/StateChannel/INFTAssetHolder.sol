// SPDX-License-Identifier: UNLICENSED
pragma solidity 0.7.6;
pragma experimental ABIEncoderV2;

/**
 * @dev Adds functionality for storing NFTs against a channel
 */
interface INFTAssetHolder {

    struct Batch {
        uint256[] ids;
        uint256[] values;
    }

    /**
     * @notice Transfers NFTs out of the channel back to the owner
     * @dev Transfers NFTs out of the channel back to the owner
     */
    function nft_transfer(
        bytes32 fromChannelId,
        address owner
    ) external;

    /**
     * @notice Returns the batch of NFTs stored against the given channel by the owner
     * @dev Returns the batch of NFTs stored against the given channel by the owner
     */
    function stored_batch(
        bytes32 channelId,
        address owner
    ) external view returns (Batch memory);

    /**
     * @dev Indicates that a batch of NFTs has been deposited by a player against a channel
     * @param channelId The channel being deposited into.
     * @param owner The address of the depositor
     * @param ids AssetIds of the NFTs within the erc1155 collection
     * @param values Number of each NFT deposited
     */
    event BatchDeposited(
        bytes32 indexed channelId,
        address owner,
        uint256[] ids,
        uint256[] values
    );

    /**
     * @dev Indicates that a batch of NFTs has been reclaimed by its owner
     * @param channelId The channel being withdrawn from
     * @param owner The address of the withdrawer
     * @param ids AssetIds of the NFTs within the erc1155 collection
     * @param values Number of each NFT withdrawn
     */
    event BatchReclaimed(
        bytes32 indexed channelId,
        address owner,
        uint256[] ids,
        uint256[] values
    );
}
