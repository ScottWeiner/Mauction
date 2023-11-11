
import Heading from '@/app/components/Heading'
import React from 'react'
import AuctionForm from '../../AuctionForm'
import { getDetailViewData } from '@/app/actions/auctionActions'



export default async function Update({ params }: { params: { id: string } }) {

    const data = await getDetailViewData(params.id)

    return (
        <div className='mx-auto max-w-[75%] shadow-lg p-10 bg-white rounded-lg'>
            <Heading title='Updat your auction' subtitle='Please update the details of your item' />
            <AuctionForm auction={data} />
        </div>
    )
}