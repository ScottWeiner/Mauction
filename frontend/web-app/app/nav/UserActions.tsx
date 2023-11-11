'use client'

import { useParamsStore } from '@/hooks/useParamsStore'
import { Button, Dropdown } from 'flowbite-react'

import { User } from 'next-auth'
import { signOut } from 'next-auth/react'
import Link from 'next/link'
import { usePathname, useRouter } from 'next/navigation'
import React from 'react'
import { AiFillCar, AiFillTrophy, AiOutlineLoading } from 'react-icons/ai'
import { HiCog, HiUser } from 'react-icons/hi2'


type Props = {
    user: User
}


export default function UserActions({ user }: Props) {

    const router = useRouter()
    const pathName = usePathname()

    const setParams = useParamsStore(state => state.setParams)

    function setWinner() {
        setParams({ winner: user.username, seller: undefined })
        console.log('Setting the Winner')
        if (pathName !== '/') {
            router.push('/')
        }
    }

    function setSeller() {
        setParams({ seller: user.username, winner: undefined })
        if (pathName !== '/') {
            router.push('/')
        }
    }

    return (
        <Dropdown label={`Welcome, ${user.name}`} inline>
            <Dropdown.Item icon={HiUser} onClick={setSeller}>
                My Acutions
            </Dropdown.Item>
            <Dropdown.Item icon={AiFillTrophy} onClick={setWinner}>
                Auctions Won
            </Dropdown.Item>
            <Dropdown.Item icon={AiFillCar}>
                <Link href='/auctions/create'>Sell an Item</Link>
            </Dropdown.Item>
            <Dropdown.Item icon={HiCog}>
                <Link href='/session'>Session (Dev Only)</Link>
            </Dropdown.Item>
            <Dropdown.Divider />
            <Dropdown.Item icon={AiOutlineLoading} onClick={() => signOut({ callbackUrl: '/' })}>Sign out</Dropdown.Item>
        </Dropdown>
    )
}
