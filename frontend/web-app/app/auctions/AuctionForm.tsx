'use client'
import { Button, TextInput } from 'flowbite-react'
import React, { useEffect } from 'react'
import { FieldValues, useForm } from 'react-hook-form'
import Input from '../components/Input'
import DateInput from '../components/DateInput'
import { createAuction, updateAuction } from '../actions/auctionActions'
import { usePathname, useRouter } from 'next/navigation'
import toast from 'react-hot-toast'
import { Auction } from '@/types'

type Props = {
    auction?: Auction
}

export default function AuctionForm({ auction }: Props) {

    const { control, reset, handleSubmit, setFocus, formState: { isSubmitting, isValid, } } = useForm({
        mode: 'onTouched'
    })

    const router = useRouter()
    const pathName = usePathname()

    async function onSubmit(data: FieldValues) {
        //console.log(data)



        try {
            let id = ''
            let res;
            if (pathName === '/auctions/create') {
                res = await createAuction(data)
                id = res.id
            } else {
                if (auction) {
                    res = await updateAuction(data, auction.id)
                    id = auction.id
                }
            }

            if (res.error) throw res.error;

            router.push(`/auctions/details/${id}`)
        } catch (error: any) {
            toast.error(error.status + ' ' + error.status)
        }

    }

    useEffect(() => {
        if (auction) {
            const { make, model, mileage, year, color } = auction
            reset({ make, model, mileage, year, color })
        }
        setFocus('make')
    }, [setFocus, auction, reset])

    return (
        <form className='flex flex-col mt-3' onSubmit={handleSubmit(onSubmit)}>
            <Input label='Make' name='make' control={control} rules={{ required: 'Make is required.' }} />
            <Input label='Model' name='model' control={control} rules={{ required: 'Model is required.' }} />
            <Input label='Color' name='color' control={control} rules={{ required: 'Color is required.' }} />
            <div className='grid grid-cols-2 gap-3'>
                <Input label='Year' name='year' control={control} rules={{ required: 'Year is required.' }} type='number' />
                <Input label='Mileage' name='mileage' control={control} rules={{ required: 'Mileage is required.' }} type='number' />
            </div>

            {pathName === '/auction/create' &&
                <>
                    <Input label='Image URL' name='imageUrl' control={control} rules={{ required: 'Image is required' }} />

                    <div className='grid grid-cols-2 gap-3'>
                        <Input label='Reserve Price (enter 0 if no reserve)' name='reservePrice' control={control} rules={{ required: 'Reserve Price is required' }} type='number' />
                        <DateInput
                            label='Auction end date/time'
                            name='auctionEnd'
                            control={control}
                            dateFormat='dd MMMM yyyy h:mm a'
                            rules={{ required: 'Auctio end date/time is required' }}
                        />
                    </div>
                </>
            }

            <div className='flex justify-between'>
                <Button outline color='gray'>Cancel</Button>
                <Button type='submit' isProcessing={isSubmitting} disabled={!isValid} outline color='success'>Submit</Button>
            </div>
        </form>
    )
}

